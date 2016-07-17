	.text
	.def	 @feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 1
	.intel_syntax noprefix
	.def	 _test_if;
	.scl	2;
	.type	32;
	.endef
	.globl	_test_if
	.p2align	4, 0x90
_test_if:                               # @test_if
# BB#0:
	push	ebp
	mov	ebp, esp
	sub	esp, 8
	mov	eax, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 8], eax
	cmp	dword ptr [ebp - 8], 0
	jg	LBB0_4
# BB#1:
	cmp	dword ptr [ebp - 8], 2
	jg	LBB0_3
# BB#2:
	mov	eax, dword ptr [ebp - 8]
	add	eax, 1
	mov	dword ptr [ebp - 4], eax
	jmp	LBB0_5
LBB0_3:
	mov	eax, dword ptr [ebp - 8]
	mov	dword ptr [ebp - 4], eax
	jmp	LBB0_5
LBB0_4:
	mov	eax, dword ptr [ebp - 8]
	sub	eax, 4
	mov	dword ptr [ebp - 4], eax
LBB0_5:
	mov	eax, dword ptr [ebp - 4]
	add	esp, 8
	pop	ebp
	ret

	.def	 _test_for;
	.scl	2;
	.type	32;
	.endef
	.globl	_test_for
	.p2align	4, 0x90
_test_for:                              # @test_for
# BB#0:
	push	ebp
	mov	ebp, esp
	sub	esp, 12
	mov	eax, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 4], eax
	mov	dword ptr [ebp - 12], 0
	mov	dword ptr [ebp - 8], 0
LBB1_1:                                 # =>This Inner Loop Header: Depth=1
	mov	eax, dword ptr [ebp - 8]
	cmp	eax, dword ptr [ebp - 4]
	jge	LBB1_4
# BB#2:                                 #   in Loop: Header=BB1_1 Depth=1
	mov	eax, dword ptr [ebp - 12]
	add	eax, dword ptr [ebp - 4]
	mov	dword ptr [ebp - 12], eax
# BB#3:                                 #   in Loop: Header=BB1_1 Depth=1
	mov	eax, dword ptr [ebp - 8]
	add	eax, 1
	mov	dword ptr [ebp - 8], eax
	jmp	LBB1_1
LBB1_4:
	mov	eax, dword ptr [ebp - 12]
	add	esp, 12
	pop	ebp
	ret

	.def	 _sum;
	.scl	2;
	.type	32;
	.endef
	.globl	_sum
	.p2align	4, 0x90
_sum:                                   # @sum
# BB#0:
	push	ebp
	mov	ebp, esp
	sub	esp, 20
	mov	eax, dword ptr [ebp + 8]
	lea	ecx, [_sum]
	mov	dword ptr [ebp - 8], eax
	mov	dword ptr [ebp - 12], ecx
	cmp	dword ptr [ebp - 8], 0
	jg	LBB2_2
# BB#1:
	mov	dword ptr [ebp - 4], 0
	jmp	LBB2_3
LBB2_2:
	mov	eax, dword ptr [ebp - 8]
	mov	ecx, dword ptr [ebp - 12]
	mov	edx, dword ptr [ebp - 8]
	sub	edx, 1
	mov	dword ptr [esp], edx
	mov	dword ptr [ebp - 16], eax # 4-byte Spill
	call	ecx
	mov	ecx, dword ptr [ebp - 16] # 4-byte Reload
	add	ecx, eax
	mov	dword ptr [ebp - 4], ecx
LBB2_3:
	mov	eax, dword ptr [ebp - 4]
	add	esp, 20
	pop	ebp
	ret

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
	sub	esp, 16
	mov	eax, dword ptr [ebp + 12]
	mov	ecx, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 4], eax
	mov	dword ptr [ebp - 8], ecx
	mov	eax, dword ptr [ebp - 8]
	sub	eax, 4
	mov	dword ptr [ebp - 12], eax
	mov	eax, dword ptr [ebp - 12]
	mov	ecx, eax
	add	ecx, 1
	mov	dword ptr [ebp - 12], ecx
	mov	dword ptr [ebp - 16], eax
	cmp	dword ptr [ebp - 12], 5
	setle	dl
	and	dl, 1
	movzx	eax, dl
	mov	dword ptr [ebp - 16], eax
	mov	eax, dword ptr [ebp - 16]
	add	esp, 16
	pop	ebp
	ret


