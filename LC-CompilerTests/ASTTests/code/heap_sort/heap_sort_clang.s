	.text
	.def	 @feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 1
	.intel_syntax noprefix
	.def	 _parent;
	.scl	2;
	.type	32;
	.endef
	.globl	_parent
	.p2align	4, 0x90
_parent:                                # @parent
# BB#0:
	push	ebp
	mov	ebp, esp
	push	eax
	mov	eax, dword ptr [ebp + 8]
	mov	ecx, 2
	mov	dword ptr [ebp - 4], eax
	mov	eax, dword ptr [ebp - 4]
	cdq
	idiv	ecx
	add	esp, 4
	pop	ebp
	ret

	.def	 _left;
	.scl	2;
	.type	32;
	.endef
	.globl	_left
	.p2align	4, 0x90
_left:                                  # @left
# BB#0:
	push	ebp
	mov	ebp, esp
	push	eax
	mov	eax, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 4], eax
	mov	eax, dword ptr [ebp - 4]
	shl	eax, 1
	add	esp, 4
	pop	ebp
	ret

	.def	 _right;
	.scl	2;
	.type	32;
	.endef
	.globl	_right
	.p2align	4, 0x90
_right:                                 # @right
# BB#0:
	push	ebp
	mov	ebp, esp
	push	eax
	mov	eax, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 4], eax
	mov	eax, dword ptr [ebp - 4]
	shl	eax, 1
	add	eax, 1
	add	esp, 4
	pop	ebp
	ret

	.def	 _swap;
	.scl	2;
	.type	32;
	.endef
	.globl	_swap
	.p2align	4, 0x90
_swap:                                  # @swap
# BB#0:
	push	ebp
	mov	ebp, esp
	sub	esp, 16
	mov	eax, dword ptr [ebp + 16]
	mov	ecx, dword ptr [ebp + 12]
	mov	edx, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 4], eax
	mov	dword ptr [ebp - 8], ecx
	mov	dword ptr [ebp - 12], edx
	mov	eax, dword ptr [ebp - 8]
	mov	ecx, dword ptr [ebp - 12]
	mov	eax, dword ptr [ecx + 4*eax]
	mov	dword ptr [ebp - 16], eax
	mov	eax, dword ptr [ebp - 4]
	mov	ecx, dword ptr [ebp - 12]
	mov	eax, dword ptr [ecx + 4*eax]
	mov	ecx, dword ptr [ebp - 8]
	mov	edx, dword ptr [ebp - 12]
	mov	dword ptr [edx + 4*ecx], eax
	mov	eax, dword ptr [ebp - 16]
	mov	ecx, dword ptr [ebp - 4]
	mov	edx, dword ptr [ebp - 12]
	mov	dword ptr [edx + 4*ecx], eax
	add	esp, 16
	pop	ebp
	ret

	.def	 _max_heapify;
	.scl	2;
	.type	32;
	.endef
	.globl	_max_heapify
	.p2align	4, 0x90
_max_heapify:                           # @max_heapify
# BB#0:
	push	ebp
	mov	ebp, esp
	sub	esp, 36
	mov	eax, dword ptr [ebp + 16]
	mov	ecx, dword ptr [ebp + 12]
	mov	edx, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 4], eax
	mov	dword ptr [ebp - 8], ecx
	mov	dword ptr [ebp - 12], edx
	mov	eax, dword ptr [ebp - 4]
	mov	dword ptr [esp], eax
	call	_left
	mov	dword ptr [ebp - 16], eax
	mov	eax, dword ptr [ebp - 4]
	mov	dword ptr [esp], eax
	call	_right
	mov	dword ptr [ebp - 20], eax
	mov	eax, dword ptr [ebp - 16]
	cmp	eax, dword ptr [ebp - 8]
	jge	LBB4_3
# BB#1:
	mov	eax, dword ptr [ebp - 16]
	mov	ecx, dword ptr [ebp - 12]
	mov	eax, dword ptr [ecx + 4*eax]
	mov	ecx, dword ptr [ebp - 4]
	mov	edx, dword ptr [ebp - 12]
	cmp	eax, dword ptr [edx + 4*ecx]
	jle	LBB4_3
# BB#2:
	mov	eax, dword ptr [ebp - 16]
	mov	dword ptr [ebp - 24], eax
	jmp	LBB4_4
LBB4_3:
	mov	eax, dword ptr [ebp - 4]
	mov	dword ptr [ebp - 24], eax
LBB4_4:
	mov	eax, dword ptr [ebp - 20]
	cmp	eax, dword ptr [ebp - 8]
	jge	LBB4_7
# BB#5:
	mov	eax, dword ptr [ebp - 20]
	mov	ecx, dword ptr [ebp - 12]
	mov	eax, dword ptr [ecx + 4*eax]
	mov	ecx, dword ptr [ebp - 24]
	mov	edx, dword ptr [ebp - 12]
	cmp	eax, dword ptr [edx + 4*ecx]
	jle	LBB4_7
# BB#6:
	mov	eax, dword ptr [ebp - 20]
	mov	dword ptr [ebp - 24], eax
LBB4_7:
	mov	eax, dword ptr [ebp - 24]
	cmp	eax, dword ptr [ebp - 4]
	je	LBB4_9
# BB#8:
	mov	eax, dword ptr [ebp - 24]
	mov	ecx, dword ptr [ebp - 4]
	mov	edx, dword ptr [ebp - 12]
	mov	dword ptr [esp], edx
	mov	dword ptr [esp + 4], ecx
	mov	dword ptr [esp + 8], eax
	call	_swap
	mov	eax, dword ptr [ebp - 24]
	mov	ecx, dword ptr [ebp - 8]
	mov	edx, dword ptr [ebp - 12]
	mov	dword ptr [esp], edx
	mov	dword ptr [esp + 4], ecx
	mov	dword ptr [esp + 8], eax
	call	_max_heapify
LBB4_9:
	add	esp, 36
	pop	ebp
	ret

	.def	 _build_max_heap;
	.scl	2;
	.type	32;
	.endef
	.globl	_build_max_heap
	.p2align	4, 0x90
_build_max_heap:                        # @build_max_heap
# BB#0:
	push	ebp
	mov	ebp, esp
	sub	esp, 28
	mov	eax, dword ptr [ebp + 12]
	mov	ecx, dword ptr [ebp + 8]
	mov	edx, 2
	mov	dword ptr [ebp - 4], eax
	mov	dword ptr [ebp - 8], ecx
	mov	eax, dword ptr [ebp - 4]
	mov	dword ptr [ebp - 16], edx # 4-byte Spill
	cdq
	mov	ecx, dword ptr [ebp - 16] # 4-byte Reload
	idiv	ecx
	sub	eax, 1
	mov	dword ptr [ebp - 12], eax
LBB5_1:                                 # =>This Inner Loop Header: Depth=1
	cmp	dword ptr [ebp - 12], 0
	jl	LBB5_4
# BB#2:                                 #   in Loop: Header=BB5_1 Depth=1
	mov	eax, dword ptr [ebp - 12]
	mov	ecx, dword ptr [ebp - 4]
	mov	edx, dword ptr [ebp - 8]
	mov	dword ptr [esp], edx
	mov	dword ptr [esp + 4], ecx
	mov	dword ptr [esp + 8], eax
	call	_max_heapify
# BB#3:                                 #   in Loop: Header=BB5_1 Depth=1
	mov	eax, dword ptr [ebp - 12]
	add	eax, -1
	mov	dword ptr [ebp - 12], eax
	jmp	LBB5_1
LBB5_4:
	add	esp, 28
	pop	ebp
	ret

	.def	 _heap_sort;
	.scl	2;
	.type	32;
	.endef
	.globl	_heap_sort
	.p2align	4, 0x90
_heap_sort:                             # @heap_sort
# BB#0:
	push	ebp
	mov	ebp, esp
	sub	esp, 32
	mov	eax, dword ptr [ebp + 12]
	mov	ecx, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 4], eax
	mov	dword ptr [ebp - 8], ecx
	mov	eax, dword ptr [ebp - 4]
	mov	ecx, dword ptr [ebp - 8]
	mov	dword ptr [esp], ecx
	mov	dword ptr [esp + 4], eax
	call	_build_max_heap
	mov	eax, dword ptr [ebp - 4]
	sub	eax, 1
	mov	dword ptr [ebp - 12], eax
LBB6_1:                                 # =>This Inner Loop Header: Depth=1
	cmp	dword ptr [ebp - 12], 1
	jl	LBB6_4
# BB#2:                                 #   in Loop: Header=BB6_1 Depth=1
	xor	eax, eax
	mov	ecx, dword ptr [ebp - 12]
	mov	edx, dword ptr [ebp - 8]
	mov	dword ptr [esp], edx
	mov	dword ptr [esp + 4], ecx
	mov	dword ptr [esp + 8], 0
	mov	dword ptr [ebp - 16], eax # 4-byte Spill
	call	_swap
	xor	eax, eax
	mov	ecx, dword ptr [ebp - 12]
	mov	edx, dword ptr [ebp - 8]
	mov	dword ptr [esp], edx
	mov	dword ptr [esp + 4], ecx
	mov	dword ptr [esp + 8], 0
	mov	dword ptr [ebp - 20], eax # 4-byte Spill
	call	_max_heapify
# BB#3:                                 #   in Loop: Header=BB6_1 Depth=1
	mov	eax, dword ptr [ebp - 12]
	add	eax, -1
	mov	dword ptr [ebp - 12], eax
	jmp	LBB6_1
LBB6_4:
	add	esp, 32
	pop	ebp
	ret


