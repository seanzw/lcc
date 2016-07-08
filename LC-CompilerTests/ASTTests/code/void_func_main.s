	.text
	.def	 @feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 1
	.intel_syntax noprefix
	.def	 _main;
	.scl	2;
	.type	32;
	.endef
	.globl	_main
	.p2align	4, 0x90
_main:                                  # @main
# BB#0:
	push	ebp
	mov	ebp, esp
	sub	esp, 12
	mov	eax, dword ptr [ebp + 12]
	mov	ecx, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 4], 0
	mov	dword ptr [ebp - 8], eax
	mov	dword ptr [ebp - 12], ecx
	call	_func1
	xor	eax, eax
	add	esp, 12
	pop	ebp
	ret


