 /*
    2  * MUSCLE SmartCard Development ( http://pcsclite.alioth.debian.org/pcsclite.html )
    3  *
    4  * Copyright (C) 1999-2004
    5  *  David Corcoran <corcoran@musclecard.com>
    6  * Copyright (C) 2002-2011
    7  *  Ludovic Rousseau <ludovic.rousseau@free.fr>
    8  *
    9 Redistribution and use in source and binary forms, with or without
   10 modification, are permitted provided that the following conditions
   11 are met:
   12 
   13 1. Redistributions of source code must retain the above copyright
   14    notice, this list of conditions and the following disclaimer.
   15 2. Redistributions in binary form must reproduce the above copyright
   16    notice, this list of conditions and the following disclaimer in the
   17    documentation and/or other materials provided with the distribution.
   18 3. The name of the author may not be used to endorse or promote products
   19    derived from this software without specific prior written permission.
   20 
   21 THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
   22 IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
   23 OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
   24 IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
   25 INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
   26 NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
   27 DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
   28 THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
   29 (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
   30 THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
   31  */
   32 
  103 #include "config.h"
  104 #include <stdlib.h>
  105 #include <sys/time.h>
  106 #include <string.h>
  107 #include <pthread.h>
  108 
  109 #include "pcscd.h"
  110 #include "winscard.h"
  111 #include "ifdhandler.h"
  112 #include "debuglog.h"
  113 #include "readerfactory.h"
  114 #include "prothandler.h"
  115 #include "ifdwrapper.h"
  116 #include "atrhandler.h"
  117 #include "sys_generic.h"
  118 #include "eventhandler.h"
  119 #include "utils.h"
  120 #include "reader.h"
  121 
  122 #undef DO_PROFILE
  123 #ifdef DO_PROFILE
  124 
  125 #ifndef FALSE
  126 #define FALSE 0
  127 #define TRUE 1
  128 #endif
  129 
  130 #define PROFILE_FILE "/tmp/pcscd_profile"
  131 #include <stdio.h>
  132 #include <sys/time.h>
  133 #include <errno.h>
  134 #include <unistd.h>
  135 
  136 struct timeval profile_time_start;
  137 FILE *fd;
  138 char profile_tty;
  139 
  140 #define PROFILE_START profile_start(__FUNCTION__);
  141 #define PROFILE_END profile_end(__FUNCTION__, __LINE__);
  142 
  143 static void profile_start(const char *f)
  144 {
  145     static char initialized = FALSE;
  146 
  147     if (!initialized)
  148     {
  149         initialized = TRUE;
  150         fd = fopen(PROFILE_FILE, "a+");
  151         if (NULL == fd)
  152         {
  153             fprintf(stderr, "\33[01;31mCan't open %s: %s\33[0m\n",
  154                 PROFILE_FILE, strerror(errno));
  155             exit(-1);
  156         }
  157         fprintf(fd, "\nStart a new profile\n");
  158         fflush(fd);
  159 
  160         if (isatty(fileno(stderr)))
  161             profile_tty = TRUE;
  162         else
  163             profile_tty = FALSE;
  164     }
  165 
  166     gettimeofday(&profile_time_start, NULL);
  167 } /* profile_start */
  168 
  169 
  170 static void profile_end(const char *f, int line)
  171 {
  172     struct timeval profile_time_end;
  173     long d;
  174 
  175     gettimeofday(&profile_time_end, NULL);
  176     d = time_sub(&profile_time_end, &profile_time_start);
  177 
  178     if (profile_tty)
  179         fprintf(stderr, "\33[01;31mRESULT %s \33[35m%ld\33[0m (%d)\n", f, d,
  180             line);
  181     fprintf(fd, "%s %ld\n", f, d);
  182     fflush(fd);
  183 } /* profile_end */
  184 
  185 #else
  186 #define PROFILE_START
  187 #define PROFILE_END
  188 #endif
  189 
  191 #define SCARD_PROTOCOL_ANY_OLD   0x1000
  192 
  193 static pthread_mutex_t LockMutex = PTHREAD_MUTEX_INITIALIZER;
  194 
  195 LONG SCardEstablishContext(DWORD dwScope, /*@unused@*/ LPCVOID pvReserved1,
  196     /*@unused@*/ LPCVOID pvReserved2, LPSCARDCONTEXT phContext)
  197 {
  198     (void)pvReserved1;
  199     (void)pvReserved2;
  200 
  201     if (dwScope != SCARD_SCOPE_USER && dwScope != SCARD_SCOPE_TERMINAL &&
  202         dwScope != SCARD_SCOPE_SYSTEM && dwScope != SCARD_SCOPE_GLOBAL)
  203     {
  204         *phContext = 0;
  205         return SCARD_E_INVALID_VALUE;
  206     }
  207 
  208     /*
  209      * Unique identifier for this server so that it can uniquely be
  210      * identified by clients and distinguished from others
  211      */
  212 
  213     *phContext = SYS_RandomInt(0, -1);
  214 
  215     Log2(PCSC_LOG_DEBUG, "Establishing Context: 0x%lX", *phContext);
  216 
  217     return SCARD_S_SUCCESS;
  218 }
  219 
  220 LONG SCardReleaseContext(SCARDCONTEXT hContext)
  221 {
  222     /*
  223      * Nothing to do here RPC layer will handle this
  224      */
  225 
  226     Log2(PCSC_LOG_DEBUG, "Releasing Context: 0x%lX", hContext);
  227 
  228     return SCARD_S_SUCCESS;
  229 }
  230 
  231 LONG SCardConnect(/*@unused@*/ SCARDCONTEXT hContext, LPCSTR szReader,
  232     DWORD dwShareMode, DWORD dwPreferredProtocols, LPSCARDHANDLE phCard,
  233     LPDWORD pdwActiveProtocol)
  234 {
  235     LONG rv;
  236     READER_CONTEXT * rContext = NULL;
  237 
  238     (void)hContext;
  239     PROFILE_START
  240 
  241     *phCard = 0;
  242 
  243     if ((dwShareMode != SCARD_SHARE_DIRECT) &&
  244             !(dwPreferredProtocols & SCARD_PROTOCOL_T0) &&
  245             !(dwPreferredProtocols & SCARD_PROTOCOL_T1) &&
  246             !(dwPreferredProtocols & SCARD_PROTOCOL_RAW) &&
  247             !(dwPreferredProtocols & SCARD_PROTOCOL_ANY_OLD))
  248         return SCARD_E_PROTO_MISMATCH;
  249 
  250     if (dwShareMode != SCARD_SHARE_EXCLUSIVE &&
  251             dwShareMode != SCARD_SHARE_SHARED &&
  252             dwShareMode != SCARD_SHARE_DIRECT)
  253         return SCARD_E_INVALID_VALUE;
  254 
  255     Log3(PCSC_LOG_DEBUG, "Attempting Connect to %s using protocol: %ld",
  256         szReader, dwPreferredProtocols);
  257 
  258     rv = RFReaderInfo((LPSTR) szReader, &rContext);
  259     if (rv != SCARD_S_SUCCESS)
  260     {
  261         Log2(PCSC_LOG_ERROR, "Reader %s Not Found", szReader);
  262         return rv;
  263     }
  264 
  265     /*
  266      * Make sure the reader is working properly
  267      */
  268     rv = RFCheckReaderStatus(rContext);
  269     if (rv != SCARD_S_SUCCESS)
  270         goto exit;
  271 
  272     /*******************************************
  273      *
  274      * This section checks for simple errors
  275      *
  276      *******************************************/
  277 
  278     /*
  279      * Connect if not exclusive mode
  280      */
  281     if (rContext->contexts == PCSCLITE_SHARING_EXCLUSIVE_CONTEXT)
  282     {
  283         Log1(PCSC_LOG_ERROR, "Error Reader Exclusive");
  284         rv = SCARD_E_SHARING_VIOLATION;
  285         goto exit;
  286     }
  287 
  288     /*
  289      * wait until a possible transaction is finished
  290      */
  291     if (rContext->hLockId != 0)
  292     {
  293         Log1(PCSC_LOG_INFO, "Waiting for release of lock");
  294         while (rContext->hLockId != 0)
  295             (void)SYS_USleep(PCSCLITE_LOCK_POLL_RATE);
  296         Log1(PCSC_LOG_INFO, "Lock released");
  297     }
  298 
  299     /*******************************************
  300      *
  301      * This section tries to determine the
  302      * presence of a card or not
  303      *
  304      *******************************************/
  305     if (dwShareMode != SCARD_SHARE_DIRECT)
  306     {
  307         if (!(rContext->readerState->readerState & SCARD_PRESENT))
  308         {
  309             Log1(PCSC_LOG_DEBUG, "Card Not Inserted");
  310             rv = SCARD_E_NO_SMARTCARD;
  311             goto exit;
  312         }
  313 
  314         /* Power on (again) the card if needed */
  315         (void)pthread_mutex_lock(&rContext->powerState_lock);
  316         if (POWER_STATE_UNPOWERED == rContext->powerState)
  317         {
  318             DWORD dwAtrLen;
  319 
  320             dwAtrLen = sizeof(rContext->readerState->cardAtr);
  321             rv = IFDPowerICC(rContext, IFD_POWER_UP,
  322                 rContext->readerState->cardAtr, &dwAtrLen);
  323             rContext->readerState->cardAtrLength = dwAtrLen;
  324 
  325             if (rv == IFD_SUCCESS)
  326             {
  327                 rContext->readerState->readerState = SCARD_PRESENT | SCARD_POWERED | SCARD_NEGOTIABLE;
  328 
  329                 Log1(PCSC_LOG_DEBUG, "power up complete.");
  330                 LogXxd(PCSC_LOG_DEBUG, "Card ATR: ",
  331                     rContext->readerState->cardAtr,
  332                     rContext->readerState->cardAtrLength);
  333             }
  334             else
  335                 Log3(PCSC_LOG_ERROR, "Error powering up card: %ld 0x%04lX",
  336                     rv, rv);
  337         }
  338 
  339         if (! (rContext->readerState->readerState & SCARD_POWERED))
  340         {
  341             Log1(PCSC_LOG_ERROR, "Card Not Powered");
  342             (void)pthread_mutex_unlock(&rContext->powerState_lock);
  343             rv = SCARD_W_UNPOWERED_CARD;
  344             goto exit;
  345         }
  346 
  347         /* the card is now in use */
  348         rContext->powerState = POWER_STATE_INUSE;
  349         Log1(PCSC_LOG_DEBUG, "powerState: POWER_STATE_INUSE");
  350         (void)pthread_mutex_unlock(&rContext->powerState_lock);
  351     }
  352 
  353     /*******************************************
  354      *
  355      * This section tries to decode the ATR
  356      * and set up which protocol to use
  357      *
  358      *******************************************/
  359     if (dwPreferredProtocols & SCARD_PROTOCOL_RAW)
  360         rContext->readerState->cardProtocol = SCARD_PROTOCOL_RAW;
  361     else
  362     {
  363         if (dwShareMode != SCARD_SHARE_DIRECT)
  364         {
  365             /* lock here instead in IFDSetPTS() to lock up to
  366              * setting rContext->readerState->cardProtocol */
  367             (void)pthread_mutex_lock(rContext->mMutex);
  368 
  369             /* the protocol is not yet set (no PPS yet) */
  370             if (SCARD_PROTOCOL_UNDEFINED == rContext->readerState->cardProtocol)
  371             {
  372                 int availableProtocols, defaultProtocol;
  373                 int ret;
  374 
  375                 ATRDecodeAtr(&availableProtocols, &defaultProtocol,
  376                     rContext->readerState->cardAtr,
  377                     rContext->readerState->cardAtrLength);
  378 
  379                 /* If it is set to ANY let it do any of the protocols */
  380                 if (dwPreferredProtocols & SCARD_PROTOCOL_ANY_OLD)
  381                     dwPreferredProtocols = SCARD_PROTOCOL_T0 | SCARD_PROTOCOL_T1;
  382 
  383                 ret = PHSetProtocol(rContext, dwPreferredProtocols,
  384                     availableProtocols, defaultProtocol);
  385 
  386                 /* keep cardProtocol = SCARD_PROTOCOL_UNDEFINED in case of error  */
  387                 if (SET_PROTOCOL_PPS_FAILED == ret)
  388                 {
  389                     (void)pthread_mutex_unlock(rContext->mMutex);
  390                     rv = SCARD_W_UNRESPONSIVE_CARD;
  391                     goto exit;
  392                 }
  393 
  394                 if (SET_PROTOCOL_WRONG_ARGUMENT == ret)
  395                 {
  396                     (void)pthread_mutex_unlock(rContext->mMutex);
  397                     rv = SCARD_E_PROTO_MISMATCH;
  398                     goto exit;
  399                 }
  400 
  401                 /* use negotiated protocol */
  402                 rContext->readerState->cardProtocol = ret;
  403 
  404                 (void)pthread_mutex_unlock(rContext->mMutex);
  405             }
  406             else
  407             {
  408                 (void)pthread_mutex_unlock(rContext->mMutex);
  409 
  410                 if (! (dwPreferredProtocols & rContext->readerState->cardProtocol))
  411                 {
  412                     rv = SCARD_E_PROTO_MISMATCH;
  413                     goto exit;
  414                 }
  415             }
  416         }
  417     }
  418 
  419     *pdwActiveProtocol = rContext->readerState->cardProtocol;
  420 
  421     if (dwShareMode != SCARD_SHARE_DIRECT)
  422     {
  423         switch (*pdwActiveProtocol)
  424         {
  425             case SCARD_PROTOCOL_T0:
  426             case SCARD_PROTOCOL_T1:
  427                 Log2(PCSC_LOG_DEBUG, "Active Protocol: T=%d",
  428                     (*pdwActiveProtocol == SCARD_PROTOCOL_T0) ? 0 : 1);
  429                 break;
  430 
  431             case SCARD_PROTOCOL_RAW:
  432                 Log1(PCSC_LOG_DEBUG, "Active Protocol: RAW");
  433                 break;
  434 
  435             default:
  436                 Log2(PCSC_LOG_ERROR, "Active Protocol: unknown %ld",
  437                     *pdwActiveProtocol);
  438         }
  439     }
  440     else
  441         Log1(PCSC_LOG_DEBUG, "Direct access: no protocol selected");
  442 
  443     /*
  444      * Prepare the SCARDHANDLE identity
  445      */
  446 
  447     /* we need a lock to avoid concurent generation of handles leading
  448      * to a possible hCard handle duplication */
  449     (void)pthread_mutex_lock(&LockMutex);
  450 
  451     *phCard = RFCreateReaderHandle(rContext);
  452 
  453     Log2(PCSC_LOG_DEBUG, "hCard Identity: %lx", *phCard);
  454 
  455     /*******************************************
  456      *
  457      * This section tries to set up the
  458      * exclusivity modes. -1 is exclusive
  459      *
  460      *******************************************/
  461 
  462     if (dwShareMode == SCARD_SHARE_EXCLUSIVE)
  463     {
  464         if (rContext->contexts == PCSCLITE_SHARING_NO_CONTEXT)
  465         {
  466             rContext->contexts = PCSCLITE_SHARING_EXCLUSIVE_CONTEXT;
  467             (void)RFLockSharing(*phCard, rContext);
  468         }
  469         else
  470         {
  471             (void)RFDestroyReaderHandle(*phCard);
  472             *phCard = 0;
  473             rv = SCARD_E_SHARING_VIOLATION;
  474             (void)pthread_mutex_unlock(&LockMutex);
  475             goto exit;
  476         }
  477     }
  478     else
  479     {
  480         /*
  481          * Add a connection to the context stack
  482          */
  483         rContext->contexts += 1;
  484     }
  485 
  486     /*
  487      * Add this handle to the handle list
  488      */
  489     rv = RFAddReaderHandle(rContext, *phCard);
  490 
  491     (void)pthread_mutex_unlock(&LockMutex);
  492 
  493     if (rv != SCARD_S_SUCCESS)
  494     {
  495         /*
  496          * Clean up - there is no more room
  497          */
  498         (void)RFDestroyReaderHandle(*phCard);
  499         if (rContext->contexts == PCSCLITE_SHARING_EXCLUSIVE_CONTEXT)
  500             rContext->contexts = PCSCLITE_SHARING_NO_CONTEXT;
  501         else
  502             if (rContext->contexts > PCSCLITE_SHARING_NO_CONTEXT)
  503                 rContext->contexts -= 1;
  504 
  505         *phCard = 0;
  506 
  507         rv = SCARD_F_INTERNAL_ERROR;
  508         goto exit;
  509     }
  510 
  511     /*
  512      * Propagate new state to reader state
  513      */
  514     rContext->readerState->readerSharing = rContext->contexts;
  515 
  516 exit:
  517     UNREF_READER(rContext)
  518 
  519     PROFILE_END
  520 
  521     return rv;
  522 }
  523 
  524 LONG SCardReconnect(SCARDHANDLE hCard, DWORD dwShareMode,
  525     DWORD dwPreferredProtocols, DWORD dwInitialization,
  526     LPDWORD pdwActiveProtocol)
  527 {
  528     LONG rv;
  529     READER_CONTEXT * rContext = NULL;
  530 
  531     Log1(PCSC_LOG_DEBUG, "Attempting reconnect to token.");
  532 
  533     if (hCard == 0)
  534         return SCARD_E_INVALID_HANDLE;
  535 
  536     /*
  537      * Handle the dwInitialization
  538      */
  539     if (dwInitialization != SCARD_LEAVE_CARD &&
  540             dwInitialization != SCARD_RESET_CARD &&
  541             dwInitialization != SCARD_UNPOWER_CARD)
  542         return SCARD_E_INVALID_VALUE;
  543 
  544     if (dwShareMode != SCARD_SHARE_SHARED &&
  545             dwShareMode != SCARD_SHARE_EXCLUSIVE &&
  546             dwShareMode != SCARD_SHARE_DIRECT)
  547         return SCARD_E_INVALID_VALUE;
  548 
  549     if ((dwShareMode != SCARD_SHARE_DIRECT) &&
  550             !(dwPreferredProtocols & SCARD_PROTOCOL_T0) &&
  551             !(dwPreferredProtocols & SCARD_PROTOCOL_T1) &&
  552             !(dwPreferredProtocols & SCARD_PROTOCOL_RAW) &&
  553             !(dwPreferredProtocols & SCARD_PROTOCOL_ANY_OLD))
  554         return SCARD_E_PROTO_MISMATCH;
  555 
  556     /* get rContext corresponding to hCard */
  557     rv = RFReaderInfoById(hCard, &rContext);
  558     if (rv != SCARD_S_SUCCESS)
  559         return rv;
  560 
  561     /*
  562      * Make sure the reader is working properly
  563      */
  564     rv = RFCheckReaderStatus(rContext);
  565     if (rv != SCARD_S_SUCCESS)
  566         goto exit;
  567 
  568     /*
  569      * Make sure no one has a lock on this reader
  570      */
  571     rv = RFCheckSharing(hCard, rContext);
  572     if (rv != SCARD_S_SUCCESS)
  573         goto exit;
  574 
  575     if (dwInitialization == SCARD_RESET_CARD ||
  576         dwInitialization == SCARD_UNPOWER_CARD)
  577     {
  578         DWORD dwAtrLen;
  579 
  580         /*
  581          * Notify the card has been reset
  582          */
  583         (void)RFSetReaderEventState(rContext, SCARD_RESET);
  584 
  585         /*
  586          * Currently pcsc-lite keeps the card powered constantly
  587          */
  588         dwAtrLen = sizeof(rContext->readerState->cardAtr);
  589         if (SCARD_RESET_CARD == dwInitialization)
  590             rv = IFDPowerICC(rContext, IFD_RESET,
  591                 rContext->readerState->cardAtr, &dwAtrLen);
  592         else
  593         {
  594             IFDPowerICC(rContext, IFD_POWER_DOWN, NULL, NULL);
  595             rv = IFDPowerICC(rContext, IFD_POWER_UP,
  596                 rContext->readerState->cardAtr, &dwAtrLen);
  597         }
  598 
  599         /* the protocol is unset after a power on */
  600         rContext->readerState->cardProtocol = SCARD_PROTOCOL_UNDEFINED;
  601 
  602         /*
  603          * Set up the status bit masks on readerState
  604          */
  605         if (rv == SCARD_S_SUCCESS)
  606         {
  607             rContext->readerState->cardAtrLength = dwAtrLen;
  608             rContext->readerState->readerState =
  609                 SCARD_PRESENT | SCARD_POWERED | SCARD_NEGOTIABLE;
  610 
  611             Log1(PCSC_LOG_DEBUG, "Reset complete.");
  612             LogXxd(PCSC_LOG_DEBUG, "Card ATR: ",
  613                 rContext->readerState->cardAtr,
  614                 rContext->readerState->cardAtrLength);
  615         }
  616         else
  617         {
  618             rContext->readerState->cardAtrLength = 0;
  619             Log1(PCSC_LOG_ERROR, "Error resetting card.");
  620 
  621             if (rv == SCARD_W_REMOVED_CARD)
  622             {
  623                 rContext->readerState->readerState = SCARD_ABSENT;
  624                 rv = SCARD_E_NO_SMARTCARD;
  625                 goto exit;
  626             }
  627             else
  628             {
  629                 rContext->readerState->readerState =
  630                     SCARD_PRESENT | SCARD_SWALLOWED;
  631                 rv = SCARD_W_UNRESPONSIVE_CARD;
  632                 goto exit;
  633             }
  634         }
  635     }
  636     else
  637         if (dwInitialization == SCARD_LEAVE_CARD)
  638         {
  639             uint32_t readerState = rContext->readerState->readerState;
  640 
  641             if (readerState & SCARD_ABSENT)
  642             {
  643                 rv = SCARD_E_NO_SMARTCARD;
  644                 goto exit;
  645             }
  646 
  647             if ((readerState & SCARD_PRESENT)
  648                 && (readerState & SCARD_SWALLOWED))
  649             {
  650                 rv = SCARD_W_UNRESPONSIVE_CARD;
  651                 goto exit;
  652             }
  653         }
  654 
  655     /*******************************************
  656      *
  657      * This section tries to decode the ATR
  658      * and set up which protocol to use
  659      *
  660      *******************************************/
  661     if (dwPreferredProtocols & SCARD_PROTOCOL_RAW)
  662         rContext->readerState->cardProtocol = SCARD_PROTOCOL_RAW;
  663     else
  664     {
  665         if (dwShareMode != SCARD_SHARE_DIRECT)
  666         {
  667             /* lock here instead in IFDSetPTS() to lock up to
  668              * setting rContext->readerState->cardProtocol */
  669             (void)pthread_mutex_lock(rContext->mMutex);
  670 
  671             /* the protocol is not yet set (no PPS yet) */
  672             if (SCARD_PROTOCOL_UNDEFINED == rContext->readerState->cardProtocol)
  673             {
  674                 int availableProtocols, defaultProtocol;
  675                 int ret;
  676 
  677                 ATRDecodeAtr(&availableProtocols, &defaultProtocol,
  678                     rContext->readerState->cardAtr,
  679                     rContext->readerState->cardAtrLength);
  680 
  681                 /* If it is set to ANY let it do any of the protocols */
  682                 if (dwPreferredProtocols & SCARD_PROTOCOL_ANY_OLD)
  683                     dwPreferredProtocols = SCARD_PROTOCOL_T0 | SCARD_PROTOCOL_T1;
  684 
  685                 ret = PHSetProtocol(rContext, dwPreferredProtocols,
  686                     availableProtocols, defaultProtocol);
  687 
  688                 /* keep cardProtocol = SCARD_PROTOCOL_UNDEFINED in case of error  */
  689                 if (SET_PROTOCOL_PPS_FAILED == ret)
  690                 {
  691                     (void)pthread_mutex_unlock(rContext->mMutex);
  692                     rv = SCARD_W_UNRESPONSIVE_CARD;
  693                     goto exit;
  694                 }
  695 
  696                 if (SET_PROTOCOL_WRONG_ARGUMENT == ret)
  697                 {
  698                     (void)pthread_mutex_unlock(rContext->mMutex);
  699                     rv = SCARD_E_PROTO_MISMATCH;
  700                     goto exit;
  701                 }
  702 
  703                 /* use negotiated protocol */
  704                 rContext->readerState->cardProtocol = ret;
  705 
  706                 (void)pthread_mutex_unlock(rContext->mMutex);
  707             }
  708             else
  709             {
  710                 (void)pthread_mutex_unlock(rContext->mMutex);
  711 
  712                 if (! (dwPreferredProtocols & rContext->readerState->cardProtocol))
  713                 {
  714                     rv = SCARD_E_PROTO_MISMATCH;
  715                     goto exit;
  716                 }
  717             }
  718         }
  719     }
  720 
  721     *pdwActiveProtocol = rContext->readerState->cardProtocol;
  722 
  723     if (dwShareMode != SCARD_SHARE_DIRECT)
  724     {
  725         switch (*pdwActiveProtocol)
  726         {
  727             case SCARD_PROTOCOL_T0:
  728             case SCARD_PROTOCOL_T1:
  729                 Log2(PCSC_LOG_DEBUG, "Active Protocol: T=%d",
  730                     (*pdwActiveProtocol == SCARD_PROTOCOL_T0) ? 0 : 1);
  731                 break;
  732 
  733             case SCARD_PROTOCOL_RAW:
  734                 Log1(PCSC_LOG_DEBUG, "Active Protocol: RAW");
  735                 break;
  736 
  737             default:
  738                 Log2(PCSC_LOG_ERROR, "Active Protocol: unknown %ld",
  739                     *pdwActiveProtocol);
  740         }
  741     }
  742     else
  743         Log1(PCSC_LOG_DEBUG, "Direct access: no protocol selected");
  744 
  745     if (dwShareMode == SCARD_SHARE_EXCLUSIVE)
  746     {
  747         if (rContext->contexts == PCSCLITE_SHARING_EXCLUSIVE_CONTEXT)
  748         {
  749             /*
  750              * Do nothing - we are already exclusive
  751              */
  752         }
  753         else
  754         {
  755             if (rContext->contexts == PCSCLITE_SHARING_LAST_CONTEXT)
  756             {
  757                 rContext->contexts = PCSCLITE_SHARING_EXCLUSIVE_CONTEXT;
  758                 (void)RFLockSharing(hCard, rContext);
  759             }
  760             else
  761             {
  762                 rv = SCARD_E_SHARING_VIOLATION;
  763                 goto exit;
  764             }
  765         }
  766     }
  767     else if (dwShareMode == SCARD_SHARE_SHARED)
  768     {
  769         if (rContext->contexts != PCSCLITE_SHARING_EXCLUSIVE_CONTEXT)
  770         {
  771             /*
  772              * Do nothing - in sharing mode already
  773              */
  774         }
  775         else
  776         {
  777             /*
  778              * We are in exclusive mode but want to share now
  779              */
  780             (void)RFUnlockSharing(hCard, rContext);
  781             rContext->contexts = PCSCLITE_SHARING_LAST_CONTEXT;
  782         }
  783     }
  784     else if (dwShareMode == SCARD_SHARE_DIRECT)
  785     {
  786         if (rContext->contexts != PCSCLITE_SHARING_EXCLUSIVE_CONTEXT)
  787         {
  788             /*
  789              * Do nothing - in sharing mode already
  790              */
  791         }
  792         else
  793         {
  794             /*
  795              * We are in exclusive mode but want to share now
  796              */
  797             (void)RFUnlockSharing(hCard, rContext);
  798             rContext->contexts = PCSCLITE_SHARING_LAST_CONTEXT;
  799         }
  800     }
  801     else
  802     {
  803         rv = SCARD_E_INVALID_VALUE;
  804         goto exit;
  805     }
  806 
  807     /*
  808      * Clear a previous event to the application
  809      */
  810     (void)RFClearReaderEventState(rContext, hCard);
  811 
  812     /*
  813      * Propagate new state to reader state
  814      */
  815     rContext->readerState->readerSharing = rContext->contexts;
  816 
  817     rv = SCARD_S_SUCCESS;
  818 
  819 exit:
  820     UNREF_READER(rContext)
  821 
  822     return rv;
  823 }
  824 
  825 LONG SCardDisconnect(SCARDHANDLE hCard, DWORD dwDisposition)
  826 {
  827     LONG rv;
  828     READER_CONTEXT * rContext = NULL;
  829 
  830     if (hCard == 0)
  831         return SCARD_E_INVALID_HANDLE;
  832 
  833     if ((dwDisposition != SCARD_LEAVE_CARD)
  834         && (dwDisposition != SCARD_UNPOWER_CARD)
  835         && (dwDisposition != SCARD_RESET_CARD)
  836         && (dwDisposition != SCARD_EJECT_CARD))
  837         return SCARD_E_INVALID_VALUE;
  838 
  839     /* get rContext corresponding to hCard */
  840     rv = RFReaderInfoById(hCard, &rContext);
  841     if (rv != SCARD_S_SUCCESS)
  842         return rv;
  843 
  844     /*
  845      * wait until a possible transaction is finished
  846      */
  847     if ((dwDisposition != SCARD_LEAVE_CARD) && (rContext->hLockId != 0)
  848         && (rContext->hLockId != hCard))
  849     {
  850         Log1(PCSC_LOG_INFO, "Waiting for release of lock");
  851         while (rContext->hLockId != 0)
  852             (void)SYS_USleep(PCSCLITE_LOCK_POLL_RATE);
  853         Log1(PCSC_LOG_INFO, "Lock released");
  854     }
  855 
  856     /*
  857      * Try to unlock any blocks on this context
  858      *
  859      * This may fail with SCARD_E_SHARING_VIOLATION if a transaction is
  860      * on going on another card context and dwDisposition == SCARD_LEAVE_CARD.
  861      * We should not stop.
  862      */
  863     rv = RFUnlockAllSharing(hCard, rContext);
  864     if (rv != SCARD_S_SUCCESS)
  865     {
  866         if (rv != SCARD_E_SHARING_VIOLATION)
  867         {
  868             goto exit;
  869         }
  870         else
  871         {
  872             if (SCARD_LEAVE_CARD != dwDisposition)
  873                 goto exit;
  874         }
  875     }
  876 
  877     Log2(PCSC_LOG_DEBUG, "Active Contexts: %d", rContext->contexts);
  878     Log2(PCSC_LOG_DEBUG, "dwDisposition: %ld", dwDisposition);
  879 
  880     if (dwDisposition == SCARD_RESET_CARD ||
  881         dwDisposition == SCARD_UNPOWER_CARD)
  882     {
  883         DWORD dwAtrLen;
  884 
  885         /*
  886          * Notify the card has been reset
  887          */
  888         (void)RFSetReaderEventState(rContext, SCARD_RESET);
  889 
  890         dwAtrLen = sizeof(rContext->readerState->cardAtr);
  891         if (SCARD_RESET_CARD == dwDisposition)
  892             rv = IFDPowerICC(rContext, IFD_RESET,
  893                 rContext->readerState->cardAtr, &dwAtrLen);
  894         else
  895         {
  896             /* SCARD_UNPOWER_CARD */
  897             IFDPowerICC(rContext, IFD_POWER_DOWN, NULL, NULL);
  898 
  899             rContext->powerState = POWER_STATE_UNPOWERED;
  900             Log1(PCSC_LOG_DEBUG, "powerState: POWER_STATE_UNPOWERED");
  901         }
  902 
  903         /* the protocol is unset after a power on */
  904         rContext->readerState->cardProtocol = SCARD_PROTOCOL_UNDEFINED;
  905 
  906         if (SCARD_UNPOWER_CARD == dwDisposition)
  907         {
  908             if (rv == SCARD_S_SUCCESS)
  909                 rContext->readerState->readerState = SCARD_PRESENT;
  910             else
  911             {
  912                 Log3(PCSC_LOG_ERROR, "Error powering down card: %ld 0x%04lX",
  913                     rv, rv);
  914                 if (rv == SCARD_W_REMOVED_CARD)
  915                     rContext->readerState->readerState = SCARD_ABSENT;
  916                 else
  917                     rContext->readerState->readerState =
  918                         SCARD_PRESENT | SCARD_SWALLOWED;
  919             }
  920         }
  921         else
  922         {
  923             /*
  924              * Set up the status bit masks on readerState
  925              */
  926             if (rv == SCARD_S_SUCCESS)
  927             {
  928                 rContext->readerState->cardAtrLength = dwAtrLen;
  929                 rContext->readerState->readerState =
  930                     SCARD_PRESENT | SCARD_POWERED | SCARD_NEGOTIABLE;
  931 
  932                 Log1(PCSC_LOG_DEBUG, "Reset complete.");
  933                 LogXxd(PCSC_LOG_DEBUG, "Card ATR: ",
  934                     rContext->readerState->cardAtr,
  935                     rContext->readerState->cardAtrLength);
  936             }
  937             else
  938             {
  939                 rContext->readerState->cardAtrLength = 0;
  940                 Log1(PCSC_LOG_ERROR, "Error resetting card.");
  941 
  942                 if (rv == SCARD_W_REMOVED_CARD)
  943                     rContext->readerState->readerState = SCARD_ABSENT;
  944                 else
  945                     rContext->readerState->readerState =
  946                         SCARD_PRESENT | SCARD_SWALLOWED;
  947             }
  948         }
  949     }
  950     else if (dwDisposition == SCARD_EJECT_CARD)
  951     {
  952         UCHAR controlBuffer[5];
  953         UCHAR receiveBuffer[MAX_BUFFER_SIZE];
  954         DWORD receiveLength;
  955 
  956         /*
  957          * Set up the CTBCS command for Eject ICC
  958          */
  959         controlBuffer[0] = 0x20;
  960         controlBuffer[1] = 0x15;
  961         controlBuffer[2] = (rContext->slot & 0x0000FFFF) + 1;
  962         controlBuffer[3] = 0x00;
  963         controlBuffer[4] = 0x00;
  964         receiveLength = 2;
  965         rv = IFDControl_v2(rContext, controlBuffer, 5, receiveBuffer,
  966             &receiveLength);
  967 
  968         if (rv == SCARD_S_SUCCESS)
  969         {
  970             if (receiveLength == 2 && receiveBuffer[0] == 0x90)
  971             {
  972                 Log1(PCSC_LOG_DEBUG, "Card ejected successfully.");
  973                 /*
  974                  * Successful
  975                  */
  976             }
  977             else
  978                 Log1(PCSC_LOG_ERROR, "Error ejecting card.");
  979         }
  980         else
  981             Log1(PCSC_LOG_ERROR, "Error ejecting card.");
  982 
  983     }
  984     else if (dwDisposition == SCARD_LEAVE_CARD)
  985     {
  986         /*
  987          * Do nothing
  988          */
  989     }
  990 
  991     /*
  992      * Remove and destroy this handle
  993      */
  994     (void)RFRemoveReaderHandle(rContext, hCard);
  995     (void)RFDestroyReaderHandle(hCard);
  996 
  997     /*
  998      * For exclusive connection reset it to no connections
  999      */
 1000     if (rContext->contexts == PCSCLITE_SHARING_EXCLUSIVE_CONTEXT)
 1001         rContext->contexts = PCSCLITE_SHARING_NO_CONTEXT;
 1002     else
 1003     {
 1004         /*
 1005          * Remove a connection from the context stack
 1006          */
 1007         rContext->contexts -= 1;
 1008 
 1009         if (rContext->contexts < 0)
 1010             rContext->contexts = 0;
 1011     }
 1012 
 1013     if (PCSCLITE_SHARING_NO_CONTEXT == rContext->contexts)
 1014     {
 1015         RESPONSECODE (*fct)(DWORD) = NULL;
 1016         DWORD dwGetSize;
 1017 
 1018         (void)pthread_mutex_lock(&rContext->powerState_lock);
 1019         /* Switch to POWER_STATE_GRACE_PERIOD unless the card was not
 1020          * powered */
 1021         if (POWER_STATE_POWERED <= rContext->powerState)
 1022         {
 1023             rContext->powerState = POWER_STATE_GRACE_PERIOD;
 1024             Log1(PCSC_LOG_DEBUG, "powerState: POWER_STATE_GRACE_PERIOD");
 1025         }
 1026 
 1027         (void)pthread_mutex_unlock(&rContext->powerState_lock);
 1028 
 1029         /* ask to stop the "polling" thread so it can be restarted using
 1030          * the correct timeout */
 1031         dwGetSize = sizeof(fct);
 1032         rv = IFDGetCapabilities(rContext, TAG_IFD_STOP_POLLING_THREAD,
 1033             &dwGetSize, (PUCHAR)&fct);
 1034 
 1035         if ((IFD_SUCCESS == rv) && (dwGetSize == sizeof(fct)))
 1036         {
 1037             Log1(PCSC_LOG_INFO, "Stopping polling thread");
 1038             fct(rContext->slot);
 1039         }
 1040     }
 1041 
 1042     /*
 1043      * Propagate new state to reader state
 1044      */
 1045     rContext->readerState->readerSharing = rContext->contexts;
 1046 
 1047     rv = SCARD_S_SUCCESS;
 1048 
 1049 exit:
 1050     UNREF_READER(rContext)
 1051 
 1052     return rv;
 1053 }
 1054 
 1055 LONG SCardBeginTransaction(SCARDHANDLE hCard)
 1056 {
 1057     LONG rv;
 1058     READER_CONTEXT * rContext;
 1059 
 1060     if (hCard == 0)
 1061         return SCARD_E_INVALID_HANDLE;
 1062 
 1063     /* get rContext corresponding to hCard */
 1064     rv = RFReaderInfoById(hCard, &rContext);
 1065     if (rv != SCARD_S_SUCCESS)
 1066         return rv;
 1067 
 1068     /*
 1069      * Make sure the reader is working properly
 1070      */
 1071     rv = RFCheckReaderStatus(rContext);
 1072     if (rv != SCARD_S_SUCCESS)
 1073         goto exit;
 1074 
 1075     /*
 1076      * Make sure some event has not occurred
 1077      */
 1078     rv = RFCheckReaderEventState(rContext, hCard);
 1079     if (rv != SCARD_S_SUCCESS)
 1080         goto exit;
 1081 
 1082     rv = RFLockSharing(hCard, rContext);
 1083 
 1084     /* if the transaction is not yet ready we sleep a bit so the client
 1085      * do not retry immediately */
 1086     if (SCARD_E_SHARING_VIOLATION == rv)
 1087         (void)SYS_USleep(PCSCLITE_LOCK_POLL_RATE);
 1088 
 1089     Log2(PCSC_LOG_DEBUG, "Status: 0x%08lX", rv);
 1090 
 1091 exit:
 1092     UNREF_READER(rContext)
 1093 
 1094     return rv;
 1095 }
 1096 
 1097 LONG SCardEndTransaction(SCARDHANDLE hCard, DWORD dwDisposition)
 1098 {
 1099     LONG rv;
 1100     LONG rv2;
 1101     READER_CONTEXT * rContext = NULL;
 1102 
 1103     /*
 1104      * Ignoring dwDisposition for now
 1105      */
 1106     if (hCard == 0)
 1107         return SCARD_E_INVALID_HANDLE;
 1108 
 1109     if ((dwDisposition != SCARD_LEAVE_CARD)
 1110         && (dwDisposition != SCARD_UNPOWER_CARD)
 1111         && (dwDisposition != SCARD_RESET_CARD)
 1112         && (dwDisposition != SCARD_EJECT_CARD))
 1113     return SCARD_E_INVALID_VALUE;
 1114 
 1115     /* get rContext corresponding to hCard */
 1116     rv = RFReaderInfoById(hCard, &rContext);
 1117     if (rv != SCARD_S_SUCCESS)
 1118         return rv;
 1119 
 1120     /*
 1121      * Make sure some event has not occurred
 1122      */
 1123     rv = RFCheckReaderEventState(rContext, hCard);
 1124     if (rv != SCARD_S_SUCCESS)
 1125         goto exit;
 1126 
 1127     /*
 1128      * Error if another transaction is ongoing and a card action is
 1129      * requested
 1130      */
 1131     if ((dwDisposition != SCARD_LEAVE_CARD) && (rContext->hLockId != 0)
 1132         && (rContext->hLockId != hCard))
 1133     {
 1134         Log1(PCSC_LOG_INFO, "No card reset within a transaction");
 1135         rv = SCARD_E_SHARING_VIOLATION;
 1136         goto exit;
 1137     }
 1138 
 1139     if (dwDisposition == SCARD_RESET_CARD ||
 1140         dwDisposition == SCARD_UNPOWER_CARD)
 1141     {
 1142         DWORD dwAtrLen;
 1143 
 1144         /*
 1145          * Currently pcsc-lite keeps the card always powered
 1146          */
 1147         dwAtrLen = sizeof(rContext->readerState->cardAtr);
 1148         if (SCARD_RESET_CARD == dwDisposition)
 1149             rv = IFDPowerICC(rContext, IFD_RESET,
 1150                 rContext->readerState->cardAtr, &dwAtrLen);
 1151         else
 1152         {
 1153             IFDPowerICC(rContext, IFD_POWER_DOWN, NULL, NULL);
 1154             rv = IFDPowerICC(rContext, IFD_POWER_UP,
 1155                 rContext->readerState->cardAtr, &dwAtrLen);
 1156         }
 1157 
 1158         /* the protocol is unset after a power on */
 1159         rContext->readerState->cardProtocol = SCARD_PROTOCOL_UNDEFINED;
 1160 
 1161         /*
 1162          * Notify the card has been reset
 1163          */
 1164         (void)RFSetReaderEventState(rContext, SCARD_RESET);
 1165 
 1166         /*
 1167          * Set up the status bit masks on readerState
 1168          */
 1169         if (rv == SCARD_S_SUCCESS)
 1170         {
 1171             rContext->readerState->cardAtrLength = dwAtrLen;
 1172             rContext->readerState->readerState =
 1173                 SCARD_PRESENT | SCARD_POWERED | SCARD_NEGOTIABLE;
 1174 
 1175             Log1(PCSC_LOG_DEBUG, "Reset complete.");
 1176             LogXxd(PCSC_LOG_DEBUG, "Card ATR: ",
 1177                 rContext->readerState->cardAtr,
 1178                 rContext->readerState->cardAtrLength);
 1179         }
 1180         else
 1181         {
 1182             rContext->readerState->cardAtrLength = 0;
 1183             Log1(PCSC_LOG_ERROR, "Error resetting card.");
 1184 
 1185             if (rv == SCARD_W_REMOVED_CARD)
 1186                 rContext->readerState->readerState = SCARD_ABSENT;
 1187             else
 1188                 rContext->readerState->readerState =
 1189                     SCARD_PRESENT | SCARD_SWALLOWED;
 1190         }
 1191     }
 1192     else if (dwDisposition == SCARD_EJECT_CARD)
 1193     {
 1194         UCHAR controlBuffer[5];
 1195         UCHAR receiveBuffer[MAX_BUFFER_SIZE];
 1196         DWORD receiveLength;
 1197 
 1198         /*
 1199          * Set up the CTBCS command for Eject ICC
 1200          */
 1201         controlBuffer[0] = 0x20;
 1202         controlBuffer[1] = 0x15;
 1203         controlBuffer[2] = (rContext->slot & 0x0000FFFF) + 1;
 1204         controlBuffer[3] = 0x00;
 1205         controlBuffer[4] = 0x00;
 1206         receiveLength = 2;
 1207         rv = IFDControl_v2(rContext, controlBuffer, 5, receiveBuffer,
 1208             &receiveLength);
 1209 
 1210         if (rv == SCARD_S_SUCCESS)
 1211         {
 1212             if (receiveLength == 2 && receiveBuffer[0] == 0x90)
 1213             {
 1214                 Log1(PCSC_LOG_DEBUG, "Card ejected successfully.");
 1215                 /*
 1216                  * Successful
 1217                  */
 1218             }
 1219             else
 1220                 Log1(PCSC_LOG_ERROR, "Error ejecting card.");
 1221         }
 1222         else
 1223             Log1(PCSC_LOG_ERROR, "Error ejecting card.");
 1224 
 1225     }
 1226     else if (dwDisposition == SCARD_LEAVE_CARD)
 1227     {
 1228         /*
 1229          * Do nothing
 1230          */
 1231     }
 1232 
 1233     /*
 1234      * Unlock any blocks on this context
 1235      */
 1236     /* we do not want to lose the previous rv value
 1237      * So we use another variable */
 1238     rv2 = RFUnlockSharing(hCard, rContext);
 1239     if (rv2 != SCARD_S_SUCCESS)
 1240         /* if rv is already in error then do not change its value */
 1241         if (rv == SCARD_S_SUCCESS)
 1242             rv = rv2;
 1243 
 1244     Log2(PCSC_LOG_DEBUG, "Status: 0x%08lX", rv);
 1245 
 1246 exit:
 1247     UNREF_READER(rContext)
 1248 
 1249     return rv;
 1250 }
 1251 
 1252 LONG SCardStatus(SCARDHANDLE hCard, LPSTR szReaderNames,
 1253     LPDWORD pcchReaderLen, LPDWORD pdwState,
 1254     LPDWORD pdwProtocol, LPBYTE pbAtr, LPDWORD pcbAtrLen)
 1255 {
 1256     LONG rv;
 1257     READER_CONTEXT * rContext = NULL;
 1258 
 1259     /* These parameters are not used by the client
 1260      * Client side code uses readerStates[] instead */
 1261     (void)szReaderNames;
 1262     (void)pcchReaderLen;
 1263     (void)pdwState;
 1264     (void)pdwProtocol;
 1265     (void)pbAtr;
 1266     (void)pcbAtrLen;
 1267 
 1268     if (hCard == 0)
 1269         return SCARD_E_INVALID_HANDLE;
 1270 
 1271     /* get rContext corresponding to hCard */
 1272     rv = RFReaderInfoById(hCard, &rContext);
 1273     if (rv != SCARD_S_SUCCESS)
 1274         return rv;
 1275 
 1276     /*
 1277      * Make sure no one has a lock on this reader
 1278      */
 1279     rv = RFCheckSharing(hCard, rContext);
 1280     if (rv != SCARD_S_SUCCESS)
 1281         goto exit;
 1282 
 1283     if (rContext->readerState->cardAtrLength > MAX_ATR_SIZE)
 1284     {
 1285         rv = SCARD_F_INTERNAL_ERROR;
 1286         goto exit;
 1287     }
 1288 
 1289     /*
 1290      * This is a client side function however the server maintains the
 1291      * list of events between applications so it must be passed through to
 1292      * obtain this event if it has occurred
 1293      */
 1294 
 1295     /*
 1296      * Make sure some event has not occurred
 1297      */
 1298     rv = RFCheckReaderEventState(rContext, hCard);
 1299     if (rv != SCARD_S_SUCCESS)
 1300         goto exit;
 1301 
 1302     /*
 1303      * Make sure the reader is working properly
 1304      */
 1305     rv = RFCheckReaderStatus(rContext);
 1306     if (rv != SCARD_S_SUCCESS)
 1307         goto exit;
 1308 
 1309 exit:
 1310     UNREF_READER(rContext)
 1311 
 1312     return rv;
 1313 }
 1314 
 1315 LONG SCardControl(SCARDHANDLE hCard, DWORD dwControlCode,
 1316     LPCVOID pbSendBuffer, DWORD cbSendLength,
 1317     LPVOID pbRecvBuffer, DWORD cbRecvLength, LPDWORD lpBytesReturned)
 1318 {
 1319     LONG rv;
 1320     READER_CONTEXT * rContext = NULL;
 1321 
 1322     /* 0 bytes returned by default */
 1323     *lpBytesReturned = 0;
 1324 
 1325     if (0 == hCard)
 1326         return SCARD_E_INVALID_HANDLE;
 1327 
 1328     /* get rContext corresponding to hCard */
 1329     rv = RFReaderInfoById(hCard, &rContext);
 1330     if (rv != SCARD_S_SUCCESS)
 1331         return rv;
 1332 
 1333     /*
 1334      * Make sure no one has a lock on this reader
 1335      */
 1336     rv = RFCheckSharing(hCard, rContext);
 1337     if (rv != SCARD_S_SUCCESS)
 1338         goto exit;
 1339 
 1340     if (IFD_HVERSION_2_0 == rContext->version)
 1341         if (NULL == pbSendBuffer || 0 == cbSendLength)
 1342         {
 1343             rv = SCARD_E_INVALID_PARAMETER;
 1344             goto exit;
 1345         }
 1346 
 1347     /*
 1348      * Make sure the reader is working properly
 1349      */
 1350     rv = RFCheckReaderStatus(rContext);
 1351     if (rv != SCARD_S_SUCCESS)
 1352         goto exit;
 1353 
 1354     if (IFD_HVERSION_2_0 == rContext->version)
 1355     {
 1356         /* we must wrap a API 3.0 client in an API 2.0 driver */
 1357         *lpBytesReturned = cbRecvLength;
 1358         rv = IFDControl_v2(rContext, (PUCHAR)pbSendBuffer,
 1359             cbSendLength, pbRecvBuffer, lpBytesReturned);
 1360     }
 1361     else
 1362         if (IFD_HVERSION_3_0 == rContext->version)
 1363             rv = IFDControl(rContext, dwControlCode, pbSendBuffer,
 1364                 cbSendLength, pbRecvBuffer, cbRecvLength, lpBytesReturned);
 1365         else
 1366             rv = SCARD_E_UNSUPPORTED_FEATURE;
 1367 
 1368 exit:
 1369     UNREF_READER(rContext)
 1370 
 1371     return rv;
 1372 }
 1373 
 1374 LONG SCardGetAttrib(SCARDHANDLE hCard, DWORD dwAttrId,
 1375     LPBYTE pbAttr, LPDWORD pcbAttrLen)
 1376 {
 1377     LONG rv;
 1378     READER_CONTEXT * rContext = NULL;
 1379 
 1380     if (0 == hCard)
 1381         return SCARD_E_INVALID_HANDLE;
 1382 
 1383     /* get rContext corresponding to hCard */
 1384     rv = RFReaderInfoById(hCard, &rContext);
 1385     if (rv != SCARD_S_SUCCESS)
 1386         return rv;
 1387 
 1388     /*
 1389      * Make sure no one has a lock on this reader
 1390      */
 1391     rv = RFCheckSharing(hCard, rContext);
 1392     if (rv != SCARD_S_SUCCESS)
 1393         goto exit;
 1394 
 1395     /*
 1396      * Make sure the reader is working properly
 1397      */
 1398     rv = RFCheckReaderStatus(rContext);
 1399     if (rv != SCARD_S_SUCCESS)
 1400         goto exit;
 1401 
 1402     /*
 1403      * Make sure some event has not occurred
 1404      */
 1405     rv = RFCheckReaderEventState(rContext, hCard);
 1406     if (rv != SCARD_S_SUCCESS)
 1407         goto exit;
 1408 
 1409     rv = IFDGetCapabilities(rContext, dwAttrId, pcbAttrLen, pbAttr);
 1410     switch(rv)
 1411     {
 1412         case IFD_SUCCESS:
 1413             rv = SCARD_S_SUCCESS;
 1414             break;
 1415         case IFD_ERROR_TAG:
 1416             /* Special case SCARD_ATTR_DEVICE_FRIENDLY_NAME as it is better
 1417              * implemented in pcscd (it knows the friendly name)
 1418              */
 1419             if ((SCARD_ATTR_DEVICE_FRIENDLY_NAME == dwAttrId)
 1420                 || (SCARD_ATTR_DEVICE_SYSTEM_NAME == dwAttrId))
 1421             {
 1422                 unsigned int len = strlen(rContext->readerState->readerName)+1;
 1423 
 1424                 if (len > *pcbAttrLen)
 1425                     rv = SCARD_E_INSUFFICIENT_BUFFER;
 1426                 else
 1427                 {
 1428                     strcpy((char *)pbAttr, rContext->readerState->readerName);
 1429                     rv = SCARD_S_SUCCESS;
 1430                 }
 1431                 *pcbAttrLen = len;
 1432             }
 1433             else
 1434                 rv = SCARD_E_UNSUPPORTED_FEATURE;
 1435             break;
 1436         case IFD_ERROR_INSUFFICIENT_BUFFER:
 1437             rv = SCARD_E_INSUFFICIENT_BUFFER;
 1438             break;
 1439         default:
 1440             rv = SCARD_E_NOT_TRANSACTED;
 1441     }
 1442 
 1443 exit:
 1444     UNREF_READER(rContext)
 1445 
 1446     return rv;
 1447 }
 1448 
 1449 LONG SCardSetAttrib(SCARDHANDLE hCard, DWORD dwAttrId,
 1450     LPCBYTE pbAttr, DWORD cbAttrLen)
 1451 {
 1452     LONG rv;
 1453     READER_CONTEXT * rContext = NULL;
 1454 
 1455     if (0 == hCard)
 1456         return SCARD_E_INVALID_HANDLE;
 1457 
 1458     /* get rContext corresponding to hCard */
 1459     rv = RFReaderInfoById(hCard, &rContext);
 1460     if (rv != SCARD_S_SUCCESS)
 1461         return rv;
 1462 
 1463     /*
 1464      * Make sure no one has a lock on this reader
 1465      */
 1466     rv = RFCheckSharing(hCard, rContext);
 1467     if (rv != SCARD_S_SUCCESS)
 1468         goto exit;
 1469 
 1470     /*
 1471      * Make sure the reader is working properly
 1472      */
 1473     rv = RFCheckReaderStatus(rContext);
 1474     if (rv != SCARD_S_SUCCESS)
 1475         goto exit;
 1476 
 1477     /*
 1478      * Make sure some event has not occurred
 1479      */
 1480     rv = RFCheckReaderEventState(rContext, hCard);
 1481     if (rv != SCARD_S_SUCCESS)
 1482         goto exit;
 1483 
 1484     rv = IFDSetCapabilities(rContext, dwAttrId, cbAttrLen, (PUCHAR)pbAttr);
 1485     if (rv == IFD_SUCCESS)
 1486         rv = SCARD_S_SUCCESS;
 1487     else
 1488         if (rv == IFD_ERROR_TAG)
 1489             rv = SCARD_E_UNSUPPORTED_FEATURE;
 1490         else
 1491             rv = SCARD_E_NOT_TRANSACTED;
 1492 
 1493 exit:
 1494     UNREF_READER(rContext)
 1495 
 1496     return rv;
 1497 }
 1498 
 1499 LONG SCardTransmit(SCARDHANDLE hCard, const SCARD_IO_REQUEST *pioSendPci,
 1500     LPCBYTE pbSendBuffer, DWORD cbSendLength,
 1501     SCARD_IO_REQUEST *pioRecvPci, LPBYTE pbRecvBuffer,
 1502     LPDWORD pcbRecvLength)
 1503 {
 1504     LONG rv;
 1505     READER_CONTEXT * rContext = NULL;
 1506     SCARD_IO_HEADER sSendPci, sRecvPci;
 1507     DWORD dwRxLength, tempRxLength;
 1508 
 1509     dwRxLength = *pcbRecvLength;
 1510     *pcbRecvLength = 0;
 1511 
 1512     if (hCard == 0)
 1513         return SCARD_E_INVALID_HANDLE;
 1514 
 1515     /*
 1516      * Must at least have 2 status words even for SCardControl
 1517      */
 1518     if (dwRxLength < 2)
 1519         return SCARD_E_INSUFFICIENT_BUFFER;
 1520 
 1521     /* get rContext corresponding to hCard */
 1522     rv = RFReaderInfoById(hCard, &rContext);
 1523     if (rv != SCARD_S_SUCCESS)
 1524         return rv;
 1525 
 1526     /*
 1527      * Make sure no one has a lock on this reader
 1528      */
 1529     rv = RFCheckSharing(hCard, rContext);
 1530     if (rv != SCARD_S_SUCCESS)
 1531         goto exit;
 1532 
 1533     /*
 1534      * Make sure the reader is working properly
 1535      */
 1536     rv = RFCheckReaderStatus(rContext);
 1537     if (rv != SCARD_S_SUCCESS)
 1538         goto exit;
 1539 
 1540     /*
 1541      * Make sure some event has not occurred
 1542      */
 1543     rv = RFCheckReaderEventState(rContext, hCard);
 1544     if (rv != SCARD_S_SUCCESS)
 1545         goto exit;
 1546 
 1547     /*
 1548      * Check for some common errors
 1549      */
 1550     if (pioSendPci->dwProtocol != SCARD_PROTOCOL_RAW)
 1551     {
 1552         if (rContext->readerState->readerState & SCARD_ABSENT)
 1553         {
 1554             rv = SCARD_E_NO_SMARTCARD;
 1555             goto exit;
 1556         }
 1557     }
 1558 
 1559     if (pioSendPci->dwProtocol != SCARD_PROTOCOL_RAW)
 1560     {
 1561         if (pioSendPci->dwProtocol != SCARD_PROTOCOL_ANY_OLD)
 1562         {
 1563             if (pioSendPci->dwProtocol != rContext->readerState->cardProtocol)
 1564             {
 1565                 rv = SCARD_E_PROTO_MISMATCH;
 1566                 goto exit;
 1567             }
 1568         }
 1569     }
 1570 
 1571     /*
 1572      * Quick fix: PC/SC starts at 1 for bit masking but the IFD_Handler
 1573      * just wants 0 or 1
 1574      */
 1575 
 1576     sSendPci.Protocol = 0; /* protocol T=0 by default */
 1577 
 1578     if (pioSendPci->dwProtocol == SCARD_PROTOCOL_T1)
 1579     {
 1580         sSendPci.Protocol = 1;
 1581     } else if (pioSendPci->dwProtocol == SCARD_PROTOCOL_RAW)
 1582     {
 1583         /*
 1584          * This is temporary ......
 1585          */
 1586         sSendPci.Protocol = SCARD_PROTOCOL_RAW;
 1587     } else if (pioSendPci->dwProtocol == SCARD_PROTOCOL_ANY_OLD)
 1588     {
 1589       /* Fix by Amira (Athena) */
 1590         unsigned long i;
 1591         unsigned long prot = rContext->readerState->cardProtocol;
 1592 
 1593         for (i = 0 ; prot != 1 ; i++)
 1594             prot >>= 1;
 1595 
 1596         sSendPci.Protocol = i;
 1597     }
 1598 
 1599     sSendPci.Length = pioSendPci->cbPciLength;
 1600 
 1601     sRecvPci.Protocol = pioRecvPci->dwProtocol;
 1602     sRecvPci.Length = pioRecvPci->cbPciLength;
 1603 
 1604     /* the protocol number is decoded a few lines above */
 1605     Log2(PCSC_LOG_DEBUG, "Send Protocol: T=%ld", sSendPci.Protocol);
 1606 
 1607     tempRxLength = dwRxLength;
 1608 
 1609     if ((pioSendPci->dwProtocol == SCARD_PROTOCOL_RAW)
 1610         && (rContext->version == IFD_HVERSION_2_0))
 1611     {
 1612         rv = IFDControl_v2(rContext, (PUCHAR) pbSendBuffer, cbSendLength,
 1613             pbRecvBuffer, &dwRxLength);
 1614     } else
 1615     {
 1616         rv = IFDTransmit(rContext, sSendPci, (PUCHAR) pbSendBuffer,
 1617             cbSendLength, pbRecvBuffer, &dwRxLength, &sRecvPci);
 1618     }
 1619 
 1620     pioRecvPci->dwProtocol = sRecvPci.Protocol;
 1621     pioRecvPci->cbPciLength = sRecvPci.Length;
 1622 
 1623     /*
 1624      * Check for any errors that might have occurred
 1625      */
 1626 
 1627     if (rv != SCARD_S_SUCCESS)
 1628     {
 1629         *pcbRecvLength = 0;
 1630         Log2(PCSC_LOG_ERROR, "Card not transacted: 0x%08lX", rv);
 1631         goto exit;
 1632     }
 1633 
 1634     /*
 1635      * Available is less than received
 1636      */
 1637     if (tempRxLength < dwRxLength)
 1638     {
 1639         *pcbRecvLength = 0;
 1640         rv = SCARD_E_INSUFFICIENT_BUFFER;
 1641         goto exit;
 1642     }
 1643 
 1644     /*
 1645      * Successful return
 1646      */
 1647     *pcbRecvLength = dwRxLength;
 1648 
 1649 exit:
 1650     UNREF_READER(rContext)
 1651 
 1652     return rv;
 1653 }
 1654 