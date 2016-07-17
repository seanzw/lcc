	.text
	.def	 @feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 1
	.intel_syntax noprefix
	.def	 _partition;
	.scl	2;
	.type	32;
	.endef
	.globl	_partition
	.p2align	4, 0x90
_partition:                             # @partition
# BB#0:
	push	ebp
	mov	ebp, esp
	sub	esp, 28
	mov	eax, dword ptr [ebp + 16]
	mov	ecx, dword ptr [ebp + 12]
	mov	edx, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 4], eax
	mov	dword ptr [ebp - 8], ecx
	mov	dword ptr [ebp - 12], edx
	mov	eax, dword ptr [ebp - 4]
	mov	ecx, dword ptr [ebp - 12]
	mov	eax, dword ptr [ecx + 4*eax]
	mov	dword ptr [ebp - 16], eax
	mov	eax, dword ptr [ebp - 8]
	sub	eax, 1
	mov	dword ptr [ebp - 20], eax
	mov	eax, dword ptr [ebp - 8]
	mov	dword ptr [ebp - 24], eax
LBB0_1:                                 # =>This Inner Loop Header: Depth=1
	mov	eax, dword ptr [ebp - 24]
	mov	ecx, dword ptr [ebp - 4]
	sub	ecx, 1
	cmp	eax, ecx
	jg	LBB0_6
# BB#2:                                 #   in Loop: Header=BB0_1 Depth=1
	mov	eax, dword ptr [ebp - 24]
	mov	ecx, dword ptr [ebp - 12]
	mov	eax, dword ptr [ecx + 4*eax]
	cmp	eax, dword ptr [ebp - 16]
	jg	LBB0_4
# BB#3:                                 #   in Loop: Header=BB0_1 Depth=1
	mov	eax, dword ptr [ebp - 20]
	add	eax, 1
	mov	dword ptr [ebp - 20], eax
	mov	eax, dword ptr [ebp - 20]
	mov	ecx, dword ptr [ebp - 12]
	mov	eax, dword ptr [ecx + 4*eax]
	mov	dword ptr [ebp - 28], eax
	mov	eax, dword ptr [ebp - 24]
	mov	ecx, dword ptr [ebp - 12]
	mov	eax, dword ptr [ecx + 4*eax]
	mov	ecx, dword ptr [ebp - 20]
	mov	edx, dword ptr [ebp - 12]
	mov	dword ptr [edx + 4*ecx], eax
	mov	eax, dword ptr [ebp - 28]
	mov	ecx, dword ptr [ebp - 24]
	mov	edx, dword ptr [ebp - 12]
	mov	dword ptr [edx + 4*ecx], eax
LBB0_4:                                 #   in Loop: Header=BB0_1 Depth=1
	jmp	LBB0_5
LBB0_5:                                 #   in Loop: Header=BB0_1 Depth=1
	mov	eax, dword ptr [ebp - 24]
	add	eax, 1
	mov	dword ptr [ebp - 24], eax
	jmp	LBB0_1
LBB0_6:
	mov	eax, dword ptr [ebp - 20]
	mov	ecx, dword ptr [ebp - 12]
	mov	eax, dword ptr [ecx + 4*eax + 4]
	mov	dword ptr [ebp - 28], eax
	mov	eax, dword ptr [ebp - 4]
	mov	ecx, dword ptr [ebp - 12]
	mov	eax, dword ptr [ecx + 4*eax]
	mov	ecx, dword ptr [ebp - 20]
	mov	edx, dword ptr [ebp - 12]
	mov	dword ptr [edx + 4*ecx + 4], eax
	mov	eax, dword ptr [ebp - 28]
	mov	ecx, dword ptr [ebp - 4]
	mov	edx, dword ptr [ebp - 12]
	mov	dword ptr [edx + 4*ecx], eax
	mov	eax, dword ptr [ebp - 20]
	add	eax, 1
	add	esp, 28
	pop	ebp
	ret

	.def	 _quick_sort;
	.scl	2;
	.type	32;
	.endef
	.globl	_quick_sort
	.p2align	4, 0x90
_quick_sort:                            # @quick_sort
# BB#0:
	push	ebp
	mov	ebp, esp
	sub	esp, 28
	mov	eax, dword ptr [ebp + 16]
	mov	ecx, dword ptr [ebp + 12]
	mov	edx, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 4], eax
	mov	dword ptr [ebp - 8], ecx
	mov	dword ptr [ebp - 12], edx
	mov	eax, dword ptr [ebp - 8]
	cmp	eax, dword ptr [ebp - 4]
	jge	LBB1_2
# BB#1:
	mov	eax, dword ptr [ebp - 4]
	mov	ecx, dword ptr [ebp - 8]
	mov	edx, dword ptr [ebp - 12]
	mov	dword ptr [esp], edx
	mov	dword ptr [esp + 4], ecx
	mov	dword ptr [esp + 8], eax
	call	_partition
	mov	dword ptr [ebp - 16], eax
	mov	eax, dword ptr [ebp - 16]
	sub	eax, 1
	mov	ecx, dword ptr [ebp - 8]
	mov	edx, dword ptr [ebp - 12]
	mov	dword ptr [esp], edx
	mov	dword ptr [esp + 4], ecx
	mov	dword ptr [esp + 8], eax
	call	_quick_sort
	mov	eax, dword ptr [ebp - 4]
	mov	ecx, dword ptr [ebp - 16]
	add	ecx, 1
	mov	edx, dword ptr [ebp - 12]
	mov	dword ptr [esp], edx
	mov	dword ptr [esp + 4], ecx
	mov	dword ptr [esp + 8], eax
	call	_quick_sort
LBB1_2:
	add	esp, 28
	pop	ebp
	ret


