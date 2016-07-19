	.text
	.def	 @feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 1
	.intel_syntax noprefix
	.def	 _stack_array_initialize;
	.scl	2;
	.type	32;
	.endef
	.globl	_stack_array_initialize
	.p2align	4, 0x90
_stack_array_initialize:                # @stack_array_initialize
# BB#0:
	push	ebp
	mov	ebp, esp
	sub	esp, 20
	mov	eax, dword ptr [ebp + 8]
	mov	ecx, 12
	mov	dword ptr [ebp - 8], eax
	mov	dword ptr [esp], 12
	mov	dword ptr [ebp - 16], ecx # 4-byte Spill
	call	_malloc
	mov	dword ptr [ebp - 12], eax
	cmp	dword ptr [ebp - 12], 0
	je	LBB0_4
# BB#1:
	mov	eax, dword ptr [ebp - 8]
	shl	eax, 2
	mov	dword ptr [esp], eax
	call	_malloc
	mov	ecx, dword ptr [ebp - 12]
	mov	dword ptr [ecx], eax
	mov	eax, dword ptr [ebp - 12]
	cmp	dword ptr [eax], 0
	je	LBB0_3
# BB#2:
	mov	eax, dword ptr [ebp - 12]
	mov	dword ptr [eax + 4], 0
	mov	eax, dword ptr [ebp - 8]
	mov	ecx, dword ptr [ebp - 12]
	mov	dword ptr [ecx + 8], eax
	mov	eax, dword ptr [ebp - 12]
	mov	dword ptr [ebp - 4], eax
	jmp	LBB0_5
LBB0_3:
	jmp	LBB0_4
LBB0_4:
	mov	dword ptr [ebp - 4], 0
LBB0_5:
	mov	eax, dword ptr [ebp - 4]
	add	esp, 20
	pop	ebp
	ret

	.def	 _stack_array_destroy;
	.scl	2;
	.type	32;
	.endef
	.globl	_stack_array_destroy
	.p2align	4, 0x90
_stack_array_destroy:                   # @stack_array_destroy
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
	mov	eax, dword ptr [eax]
	mov	dword ptr [esp], eax
	call	_free
	mov	eax, dword ptr [ebp - 4]
	mov	dword ptr [esp], eax
	call	_free
LBB1_2:
	add	esp, 8
	pop	ebp
	ret

	.def	 _stack_array_empty;
	.scl	2;
	.type	32;
	.endef
	.globl	_stack_array_empty
	.p2align	4, 0x90
_stack_array_empty:                     # @stack_array_empty
# BB#0:
	push	ebp
	mov	ebp, esp
	push	eax
	mov	eax, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 4], eax
	mov	eax, dword ptr [ebp - 4]
	cmp	dword ptr [eax + 4], 0
	sete	cl
	and	cl, 1
	movzx	eax, cl
	add	esp, 4
	pop	ebp
	ret

	.def	 _stack_array_push;
	.scl	2;
	.type	32;
	.endef
	.globl	_stack_array_push
	.p2align	4, 0x90
_stack_array_push:                      # @stack_array_push
# BB#0:
	push	ebp
	mov	ebp, esp
	push	esi
	sub	esp, 12
	mov	eax, dword ptr [ebp + 12]
	mov	ecx, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 12], eax
	mov	dword ptr [ebp - 16], ecx
	mov	eax, dword ptr [ebp - 16]
	mov	eax, dword ptr [eax + 4]
	mov	ecx, dword ptr [ebp - 16]
	cmp	eax, dword ptr [ecx + 8]
	jge	LBB3_2
# BB#1:
	mov	eax, dword ptr [ebp - 12]
	mov	ecx, dword ptr [ebp - 16]
	mov	edx, dword ptr [ecx + 4]
	mov	esi, edx
	add	esi, 1
	mov	dword ptr [ecx + 4], esi
	mov	ecx, dword ptr [ebp - 16]
	mov	ecx, dword ptr [ecx]
	mov	dword ptr [ecx + 4*edx], eax
	mov	dword ptr [ebp - 8], 0
	jmp	LBB3_3
LBB3_2:
	mov	dword ptr [ebp - 8], -1
LBB3_3:
	mov	eax, dword ptr [ebp - 8]
	add	esp, 12
	pop	esi
	pop	ebp
	ret

	.def	 _stack_array_pop;
	.scl	2;
	.type	32;
	.endef
	.globl	_stack_array_pop
	.p2align	4, 0x90
_stack_array_pop:                       # @stack_array_pop
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
	call	_stack_array_empty
	cmp	eax, 0
	je	LBB4_2
# BB#1:
	mov	dword ptr [ebp - 4], -1
	jmp	LBB4_3
LBB4_2:
	mov	eax, dword ptr [ebp - 12]
	mov	ecx, dword ptr [eax + 4]
	mov	edx, ecx
	add	edx, -1
	mov	dword ptr [eax + 4], edx
	mov	eax, dword ptr [ebp - 12]
	mov	eax, dword ptr [eax]
	mov	eax, dword ptr [eax + 4*ecx - 4]
	mov	ecx, dword ptr [ebp - 8]
	mov	dword ptr [ecx], eax
	mov	dword ptr [ebp - 4], 0
LBB4_3:
	mov	eax, dword ptr [ebp - 4]
	add	esp, 16
	pop	ebp
	ret


