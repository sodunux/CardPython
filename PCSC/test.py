import ctypes
Objdll = ctypes.windll.LoadLibrary("WinScard.dll")  
print Objdll.SCardReconnect
print Objdll.SCardConnectW
print Objdll.SCardConnectA
print Objdll.SCardListReadersA
print Objdll.SCardListReadersW
print Objdll.SCardEstablishContext
print Objdll.SCardListReaderGroupsA
print Objdll.SCardListCardsA
print Objdll.SCardListCardsW
print Objdll.SCardListInterfacesA
print Objdll.SCardListInterfacesW
print Objdll.SCardGetProviderIdA
print Objdll.SCardGetProviderIdW
print Objdll.SCardGetCardTypeProviderNameA
print Objdll.SCardGetCardTypeProviderNameW
print Objdll.SCardIntroduceReaderGroupA
print Objdll.SCardIntroduceReaderGroupW
print Objdll.SCardForgetReaderGroupA
print Objdll.SCardForgetReaderGroupW
print Objdll.SCardAddReaderToGroupA
print Objdll.SCardAddReaderToGroupW