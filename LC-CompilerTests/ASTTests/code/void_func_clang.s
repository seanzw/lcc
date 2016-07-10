	.text
	.def	 @feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 1
	.intel_syntax noprefix
	.def	 _func1;
	.scl	2;
	.type	32;
	.endef
	.globl	_func1
	.p2align	4, 0x90
_func1:                                 # @func1
# BB#0:
	push	ebp
	mov	ebp, esp
	sub	esp, 92
	mov	eax, dword ptr [ebp + 12]
	mov	ecx, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 4], eax
	mov	dword ptr [ebp - 8], ecx
	add	esp, 92
	pop	ebp
	ret

	.def	 _func2;
	.scl	2;
	.type	32;
	.endef
	.globl	_func2
	.p2align	4, 0x90
_func2:                                 # @func2
# BB#0:
	push	ebp
	mov	ebp, esp
	sub	esp, 12
	mov	eax, dword ptr [ebp + 12]
	mov	ecx, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 4], eax
	mov	dword ptr [ebp - 8], ecx
	add	esp, 12
	pop	ebp
	ret


