	.text
	.def	 @feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 1
	.intel_syntax noprefix
	.def	 _uchar2schar;
	.scl	2;
	.type	32;
	.endef
	.globl	_uchar2schar
	.p2align	4, 0x90
_uchar2schar:                           # @uchar2schar
# BB#0:
	push	ebp
	mov	ebp, esp
	push	eax
	mov	al, byte ptr [ebp + 8]
	mov	byte ptr [ebp - 1], al
	movsx	eax, byte ptr [ebp - 1]
	add	esp, 4
	pop	ebp
	ret

	.def	 _uint2int;
	.scl	2;
	.type	32;
	.endef
	.globl	_uint2int
	.p2align	4, 0x90
_uint2int:                              # @uint2int
# BB#0:
	push	ebp
	mov	ebp, esp
	push	eax
	mov	eax, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 4], eax
	mov	eax, dword ptr [ebp - 4]
	add	esp, 4
	pop	ebp
	ret


