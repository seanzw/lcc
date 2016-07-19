	.data
	.bss
	.text
	.intel_syntax noprefix
	# Frame Size: 0
	# EBP        UID   SYMBOL     TYPE                
	# 8          0     x          unsigned char       
	.globl _uchar2schar
_uchar2schar:
	push   ebp
	mov    ebp, esp
	sub    esp, 0
	# return (signed char)(x)
	# (signed char)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	mov    al, byte ptr [eax + 0]
	jmp    __uchar2schar_return
__uchar2schar_return:
	add    esp, 0
	pop    ebp
	ret
	# Frame Size: 0
	# EBP        UID   SYMBOL     TYPE                
	# 8          0     x          unsigned int        
	.globl _uint2int
_uint2int:
	push   ebp
	mov    ebp, esp
	sub    esp, 0
	# return (int)(x)
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	jmp    __uint2int_return
__uint2int_return:
	add    esp, 0
	pop    ebp
	ret
