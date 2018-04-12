/*
    2  * MUSCLE SmartCard Development ( http://pcsclite.alioth.debian.org/pcsclite.html )
    3  *
    4  * Copyright (C) 1999-2003
    5  *  David Corcoran <corcoran@musclecard.com>
    6  * Copyright (C) 2002-2009
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
   38 #ifndef __winscard_h__
   39 #define __winscard_h__
   40 
   41 #include <pcsclite.h>
   42 
   43 #ifdef __cplusplus
   44 extern "C"
   45 {
   46 #endif
   47 
   48 #ifndef PCSC_API
   49 #define PCSC_API
   50 #endif
   51 
   52     PCSC_API LONG SCardEstablishContext(DWORD dwScope,
   53         /*@null@*/ LPCVOID pvReserved1, /*@null@*/ LPCVOID pvReserved2,
   54         /*@out@*/ LPSCARDCONTEXT phContext);
   55 
   56     PCSC_API LONG SCardReleaseContext(SCARDCONTEXT hContext);
   57 
   58     PCSC_API LONG SCardIsValidContext(SCARDCONTEXT hContext);
   59 
   60     PCSC_API LONG SCardConnect(SCARDCONTEXT hContext,
   61         LPCSTR szReader,
   62         DWORD dwShareMode,
   63         DWORD dwPreferredProtocols,
   64         /*@out@*/ LPSCARDHANDLE phCard, /*@out@*/ LPDWORD pdwActiveProtocol);
   65 
   66     PCSC_API LONG SCardReconnect(SCARDHANDLE hCard,
   67         DWORD dwShareMode,
   68         DWORD dwPreferredProtocols,
   69         DWORD dwInitialization, /*@out@*/ LPDWORD pdwActiveProtocol);
   70 
   71     PCSC_API LONG SCardDisconnect(SCARDHANDLE hCard, DWORD dwDisposition);
   72 
   73     PCSC_API LONG SCardBeginTransaction(SCARDHANDLE hCard);
   74 
   75     PCSC_API LONG SCardEndTransaction(SCARDHANDLE hCard, DWORD dwDisposition);
   76 
   77     PCSC_API LONG SCardStatus(SCARDHANDLE hCard,
   78         /*@null@*/ /*@out@*/ LPSTR mszReaderName,
   79         /*@null@*/ /*@out@*/ LPDWORD pcchReaderLen,
   80         /*@null@*/ /*@out@*/ LPDWORD pdwState,
   81         /*@null@*/ /*@out@*/ LPDWORD pdwProtocol,
   82         /*@null@*/ /*@out@*/ LPBYTE pbAtr,
   83         /*@null@*/ /*@out@*/ LPDWORD pcbAtrLen);
   84 
   85     PCSC_API LONG SCardGetStatusChange(SCARDCONTEXT hContext,
   86         DWORD dwTimeout,
   87         SCARD_READERSTATE *rgReaderStates, DWORD cReaders);
   88 
   89     PCSC_API LONG SCardControl(SCARDHANDLE hCard, DWORD dwControlCode,
   90         LPCVOID pbSendBuffer, DWORD cbSendLength,
   91         /*@out@*/ LPVOID pbRecvBuffer, DWORD cbRecvLength,
   92         LPDWORD lpBytesReturned);
   93 
   94     PCSC_API LONG SCardTransmit(SCARDHANDLE hCard,
   95         const SCARD_IO_REQUEST *pioSendPci,
   96         LPCBYTE pbSendBuffer, DWORD cbSendLength,
   97         /*@out@*/ SCARD_IO_REQUEST *pioRecvPci,
   98         /*@out@*/ LPBYTE pbRecvBuffer, LPDWORD pcbRecvLength);
   99 
  100     PCSC_API LONG SCardListReaderGroups(SCARDCONTEXT hContext,
  101         /*@out@*/ LPSTR mszGroups, LPDWORD pcchGroups);
  102 
  103     PCSC_API LONG SCardListReaders(SCARDCONTEXT hContext,
  104         /*@null@*/ /*@out@*/ LPCSTR mszGroups,
  105         /*@null@*/ /*@out@*/ LPSTR mszReaders,
  106         /*@out@*/ LPDWORD pcchReaders);
  107 
  108     PCSC_API LONG SCardFreeMemory(SCARDCONTEXT hContext, LPCVOID pvMem);
  109 
  110     PCSC_API LONG SCardCancel(SCARDCONTEXT hContext);
  111 
  112     PCSC_API LONG SCardGetAttrib(SCARDHANDLE hCard, DWORD dwAttrId,
  113         /*@out@*/ LPBYTE pbAttr, LPDWORD pcbAttrLen);
  114 
  115     PCSC_API LONG SCardSetAttrib(SCARDHANDLE hCard, DWORD dwAttrId,
  116         LPCBYTE pbAttr, DWORD cbAttrLen);
  117 
  118 #ifdef __cplusplus
  119 }
  120 #endif
  121 
  122 #endif