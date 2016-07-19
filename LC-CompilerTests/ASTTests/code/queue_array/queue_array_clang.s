	.text
	.def	 @feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 1
	.intel_syntax noprefix
	.def	 _queue_init;
	.scl	2;
	.type	32;
	.endef
	.globl	_queue_init
	.p2align	4, 0x90
_queue_init:                            # @queue_init
# BB#0:
	push	ebp
	mov	ebp, esp
	sub	esp, 16
	mov	eax, dword ptr [ebp + 8]
	mov	ecx, 16
	mov	dword ptr [ebp - 4], eax
	mov	dword ptr [esp], 16
	mov	dword ptr [ebp - 12], ecx # 4-byte Spill
	call	_malloc
	mov	dword ptr [ebp - 8], eax
	cmp	dword ptr [ebp - 8], 0
	je	LBB0_5
# BB#1:
	mov	eax, dword ptr [ebp - 4]
	add	eax, 1
	shl	eax, 2
	mov	dword ptr [esp], eax
	call	_malloc
	mov	ecx, dword ptr [ebp - 8]
	mov	dword ptr [ecx], eax
	mov	eax, dword ptr [ebp - 8]
	cmp	dword ptr [eax], 0
	je	LBB0_3
# BB#2:
	mov	eax, dword ptr [ebp - 4]
	add	eax, 1
	mov	ecx, dword ptr [ebp - 8]
	mov	dword ptr [ecx + 12], eax
	mov	eax, dword ptr [ebp - 8]
	mov	dword ptr [eax + 8], 0
	mov	eax, dword ptr [ebp - 8]
	mov	dword ptr [eax + 4], 0
	jmp	LBB0_4
LBB0_3:
	mov	eax, dword ptr [ebp - 8]
	mov	dword ptr [esp], eax
	call	_free
LBB0_4:
	jmp	LBB0_5
LBB0_5:
	mov	eax, dword ptr [ebp - 8]
	add	esp, 16
	pop	ebp
	ret

	.def	 _queue_free;
	.scl	2;
	.type	32;
	.endef
	.globl	_queue_free
	.p2align	4, 0x90
_queue_free:                            # @queue_free
# BB#0:
	push	ebp
	mov	ebp, esp
	sub	esp, 8
	mov	eax, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 4], eax
	cmp	dword ptr [ebp - 4], 0
	je	LBB1_2
# BB#1:
	mov	eax, dword ptr [ebp - 4]
	mov	dword ptr [esp], eax
	call	_free
LBB1_2:
	add	esp, 8
	pop	ebp
	ret

	.def	 _queue_is_empty;
	.scl	2;
	.type	32;
	.endef
	.globl	_queue_is_empty
	.p2align	4, 0x90
_queue_is_empty:                        # @queue_is_empty
# BB#0:
	push	ebp
	mov	ebp, esp
	push	eax
	mov	eax, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 4], eax
	mov	eax, dword ptr [ebp - 4]
	mov	eax, dword ptr [eax + 4]
	mov	ecx, dword ptr [ebp - 4]
	cmp	eax, dword ptr [ecx + 8]
	sete	dl
	and	dl, 1
	movzx	eax, dl
	add	esp, 4
	pop	ebp
	ret

	.def	 _queue_enqueue;
	.scl	2;
	.type	32;
	.endef
	.globl	_queue_enqueue
	.p2align	4, 0x90
_queue_enqueue:                         # @queue_enqueue
# BB#0:
	push	ebp
	mov	ebp, esp
	sub	esp, 16
	mov	eax, dword ptr [ebp + 12]
	mov	ecx, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 8], eax
	mov	dword ptr [ebp - 12], ecx
	mov	eax, dword ptr [ebp - 12]
	mov	eax, dword ptr [eax + 8]
	add	eax, 1
	mov	ecx, dword ptr [ebp - 12]
	xor	edx, edx
	div	dword ptr [ecx + 12]
	mov	dword ptr [ebp - 16], edx
	mov	ecx, dword ptr [ebp - 16]
	mov	edx, dword ptr [ebp - 12]
	cmp	ecx, dword ptr [edx + 4]
	jne	LBB3_2
# BB#1:
	mov	dword ptr [ebp - 4], -1
	jmp	LBB3_3
LBB3_2:
	mov	eax, dword ptr [ebp - 8]
	mov	ecx, dword ptr [ebp - 12]
	mov	ecx, dword ptr [ecx + 8]
	mov	edx, dword ptr [ebp - 12]
	mov	edx, dword ptr [edx]
	mov	dword ptr [edx + 4*ecx], eax
	mov	eax, dword ptr [ebp - 16]
	mov	ecx, dword ptr [ebp - 12]
	mov	dword ptr [ecx + 8], eax
	mov	dword ptr [ebp - 4], 0
LBB3_3:
	mov	eax, dword ptr [ebp - 4]
	add	esp, 16
	pop	ebp
	ret

	.def	 _queue_dequeue;
	.scl	2;
	.type	32;
	.endef
	.globl	_queue_dequeue
	.p2align	4, 0x90
_queue_dequeue:                         # @queue_dequeue
# BB#0:
	push	ebp
	mov	ebp, esp
	sub	esp, 16
	mov	eax, dword ptr [ebp + 12]
	mov	ecx, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 8], eax
	mov	dword ptr [ebp - 12], ecx
	mov	eax, dword ptr [ebp - 12]
	mov	dword ptr [esp], eax
	call	_queue_is_empty
	cmp	eax, 0
	je	LBB4_2
# BB#1:
	mov	dword ptr [ebp - 4], -1
	jmp	LBB4_3
LBB4_2:
	mov	eax, dword ptr [ebp - 12]
	mov	eax, dword ptr [eax + 4]
	mov	ecx, dword ptr [ebp - 12]
	mov	ecx, dword ptr [ecx]
	mov	eax, dword ptr [ecx + 4*eax]
	mov	ecx, dword ptr [ebp - 8]
	mov	dword ptr [ecx], eax
	mov	eax, dword ptr [ebp - 12]
	mov	eax, dword ptr [eax + 4]
	add	eax, 1
	mov	ecx, dword ptr [ebp - 12]
	xor	edx, edx
	div	dword ptr [ecx + 12]
	mov	ecx, dword ptr [ebp - 12]
	mov	dword ptr [ecx + 4], edx
	mov	dword ptr [ebp - 4], 0
LBB4_3:
	mov	eax, dword ptr [ebp - 4]
	add	esp, 16
	pop	ebp
	ret


