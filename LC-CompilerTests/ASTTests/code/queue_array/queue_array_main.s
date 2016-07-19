	.text
	.def	 @feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 1
	.intel_syntax noprefix
	.def	 _sprintf;
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,_sprintf
	.globl	_sprintf
	.p2align	4, 0x90
_sprintf:                               # @sprintf
# BB#0:
	push	ebp
	mov	ebp, esp
	push	esi
	sub	esp, 32
	mov	eax, dword ptr [ebp + 12]
	mov	ecx, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 8], eax
	mov	dword ptr [ebp - 12], ecx
	lea	eax, [ebp + 16]
	mov	dword ptr [ebp - 20], eax
	mov	ecx, dword ptr [ebp - 8]
	mov	edx, dword ptr [ebp - 12]
	mov	esi, esp
	mov	dword ptr [esi + 12], eax
	mov	dword ptr [esi + 4], ecx
	mov	dword ptr [esi], edx
	mov	dword ptr [esi + 8], 0
	call	__vsprintf_l
	mov	dword ptr [ebp - 16], eax
	add	esp, 32
	pop	esi
	pop	ebp
	ret

	.def	 _vsprintf;
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,_vsprintf
	.globl	_vsprintf
	.p2align	4, 0x90
_vsprintf:                              # @vsprintf
# BB#0:
	push	ebp
	mov	ebp, esp
	push	edi
	push	esi
	sub	esp, 40
	mov	eax, dword ptr [ebp + 16]
	mov	ecx, dword ptr [ebp + 12]
	mov	edx, dword ptr [ebp + 8]
	mov	esi, 4294967295
	xor	edi, edi
	mov	dword ptr [ebp - 12], eax
	mov	dword ptr [ebp - 16], ecx
	mov	dword ptr [ebp - 20], edx
	mov	eax, dword ptr [ebp - 12]
	mov	ecx, dword ptr [ebp - 16]
	mov	edx, dword ptr [ebp - 20]
	mov	dword ptr [esp], edx
	mov	dword ptr [esp + 4], -1
	mov	dword ptr [esp + 8], ecx
	mov	dword ptr [esp + 12], 0
	mov	dword ptr [esp + 16], eax
	mov	dword ptr [ebp - 24], edi # 4-byte Spill
	mov	dword ptr [ebp - 28], esi # 4-byte Spill
	call	__vsnprintf_l
	add	esp, 40
	pop	esi
	pop	edi
	pop	ebp
	ret

	.def	 __snprintf;
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,__snprintf
	.globl	__snprintf
	.p2align	4, 0x90
__snprintf:                             # @_snprintf
# BB#0:
	push	ebp
	mov	ebp, esp
	push	edi
	push	esi
	sub	esp, 36
	mov	eax, dword ptr [ebp + 16]
	mov	ecx, dword ptr [ebp + 12]
	mov	edx, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 12], eax
	mov	dword ptr [ebp - 16], ecx
	mov	dword ptr [ebp - 20], edx
	lea	eax, [ebp + 20]
	mov	dword ptr [ebp - 28], eax
	mov	ecx, dword ptr [ebp - 12]
	mov	edx, dword ptr [ebp - 16]
	mov	esi, dword ptr [ebp - 20]
	mov	edi, esp
	mov	dword ptr [edi + 12], eax
	mov	dword ptr [edi + 8], ecx
	mov	dword ptr [edi + 4], edx
	mov	dword ptr [edi], esi
	call	__vsnprintf
	mov	dword ptr [ebp - 24], eax
	add	esp, 36
	pop	esi
	pop	edi
	pop	ebp
	ret

	.def	 __vsnprintf;
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,__vsnprintf
	.globl	__vsnprintf
	.p2align	4, 0x90
__vsnprintf:                            # @_vsnprintf
# BB#0:
	push	ebp
	mov	ebp, esp
	push	edi
	push	esi
	sub	esp, 40
	mov	eax, dword ptr [ebp + 20]
	mov	ecx, dword ptr [ebp + 16]
	mov	edx, dword ptr [ebp + 12]
	mov	esi, dword ptr [ebp + 8]
	xor	edi, edi
	mov	dword ptr [ebp - 12], eax
	mov	dword ptr [ebp - 16], ecx
	mov	dword ptr [ebp - 20], edx
	mov	dword ptr [ebp - 24], esi
	mov	eax, dword ptr [ebp - 12]
	mov	ecx, dword ptr [ebp - 16]
	mov	edx, dword ptr [ebp - 20]
	mov	esi, dword ptr [ebp - 24]
	mov	dword ptr [esp], esi
	mov	dword ptr [esp + 4], edx
	mov	dword ptr [esp + 8], ecx
	mov	dword ptr [esp + 12], 0
	mov	dword ptr [esp + 16], eax
	mov	dword ptr [ebp - 28], edi # 4-byte Spill
	call	__vsnprintf_l
	add	esp, 40
	pop	esi
	pop	edi
	pop	ebp
	ret

	.def	 _main;
	.scl	2;
	.type	32;
	.endef
	.text
	.globl	_main
	.p2align	4, 0x90
_main:                                  # @main
# BB#0:
	push	ebp
	mov	ebp, esp
	sub	esp, 136
	mov	eax, dword ptr [ebp + 12]
	mov	ecx, dword ptr [ebp + 8]
	mov	edx, 4
	mov	dword ptr [ebp - 4], 0
	mov	dword ptr [ebp - 8], eax
	mov	dword ptr [ebp - 12], ecx
	mov	eax, dword ptr [L_main.a]
	mov	dword ptr [ebp - 28], eax
	mov	eax, dword ptr [L_main.a+4]
	mov	dword ptr [ebp - 24], eax
	mov	eax, dword ptr [L_main.a+8]
	mov	dword ptr [ebp - 20], eax
	mov	eax, dword ptr [L_main.a+12]
	mov	dword ptr [ebp - 16], eax
	mov	dword ptr [esp], 4
	mov	dword ptr [ebp - 60], edx # 4-byte Spill
	call	_queue_init
	mov	dword ptr [ebp - 32], eax
	cmp	dword ptr [ebp - 32], 0
	jne	LBB4_2
# BB#1:
	lea	eax, ["??_C@_0BB@BLBNCOHH@assert?5fail?3?5?$CFs?6?$AA@"]
	lea	ecx, ["??_C@_04HKFDNBLD@init?$AA@"]
	mov	dword ptr [esp], eax
	mov	dword ptr [esp + 4], ecx
	call	_printf
	mov	dword ptr [ebp - 4], -1
	mov	dword ptr [ebp - 64], eax # 4-byte Spill
	jmp	LBB4_45
LBB4_2:
	mov	eax, dword ptr [ebp - 32]
	mov	dword ptr [esp], eax
	call	_queue_is_empty
	cmp	eax, 0
	jne	LBB4_4
# BB#3:
	lea	eax, ["??_C@_0BB@BLBNCOHH@assert?5fail?3?5?$CFs?6?$AA@"]
	lea	ecx, ["??_C@_05LBJMNBOG@empty?$AA@"]
	mov	dword ptr [esp], eax
	mov	dword ptr [esp + 4], ecx
	call	_printf
	mov	dword ptr [ebp - 4], -1
	mov	dword ptr [ebp - 68], eax # 4-byte Spill
	jmp	LBB4_45
LBB4_4:
	mov	eax, dword ptr [ebp - 32]
	cmp	dword ptr [eax + 12], 5
	je	LBB4_6
# BB#5:
	lea	eax, ["??_C@_0BB@BLBNCOHH@assert?5fail?3?5?$CFs?6?$AA@"]
	lea	ecx, ["??_C@_04IAGNFIBA@size?$AA@"]
	mov	dword ptr [esp], eax
	mov	dword ptr [esp + 4], ecx
	call	_printf
	mov	dword ptr [ebp - 4], -1
	mov	dword ptr [ebp - 72], eax # 4-byte Spill
	jmp	LBB4_45
LBB4_6:
	mov	dword ptr [ebp - 36], 0
LBB4_7:                                 # =>This Inner Loop Header: Depth=1
	cmp	dword ptr [ebp - 36], 4
	jge	LBB4_12
# BB#8:                                 #   in Loop: Header=BB4_7 Depth=1
	mov	eax, dword ptr [ebp - 36]
	mov	eax, dword ptr [ebp + 4*eax - 28]
	mov	ecx, dword ptr [ebp - 32]
	mov	dword ptr [esp], ecx
	mov	dword ptr [esp + 4], eax
	call	_queue_enqueue
	cmp	eax, 0
	je	LBB4_10
# BB#9:
	lea	eax, ["??_C@_0BB@BLBNCOHH@assert?5fail?3?5?$CFs?6?$AA@"]
	lea	ecx, ["??_C@_07GOFLDKEC@enqueue?$AA@"]
	mov	dword ptr [esp], eax
	mov	dword ptr [esp + 4], ecx
	call	_printf
	mov	dword ptr [ebp - 4], -1
	mov	dword ptr [ebp - 76], eax # 4-byte Spill
	jmp	LBB4_45
LBB4_10:                                #   in Loop: Header=BB4_7 Depth=1
	jmp	LBB4_11
LBB4_11:                                #   in Loop: Header=BB4_7 Depth=1
	mov	eax, dword ptr [ebp - 36]
	add	eax, 1
	mov	dword ptr [ebp - 36], eax
	jmp	LBB4_7
LBB4_12:
	mov	eax, dword ptr [ebp - 32]
	mov	eax, dword ptr [eax + 8]
	mov	ecx, dword ptr [ebp - 32]
	mov	ecx, dword ptr [ecx + 12]
	sub	ecx, 1
	cmp	eax, ecx
	je	LBB4_14
# BB#13:
	lea	eax, ["??_C@_0BB@BLBNCOHH@assert?5fail?3?5?$CFs?6?$AA@"]
	lea	ecx, ["??_C@_04PLMLMMEO@full?$AA@"]
	mov	dword ptr [esp], eax
	mov	dword ptr [esp + 4], ecx
	call	_printf
	mov	dword ptr [ebp - 4], -1
	mov	dword ptr [ebp - 80], eax # 4-byte Spill
	jmp	LBB4_45
LBB4_14:
	mov	eax, 1
	mov	ecx, dword ptr [ebp - 32]
	mov	dword ptr [esp], ecx
	mov	dword ptr [esp + 4], 1
	mov	dword ptr [ebp - 84], eax # 4-byte Spill
	call	_queue_enqueue
	cmp	eax, -1
	je	LBB4_16
# BB#15:
	lea	eax, ["??_C@_0BB@BLBNCOHH@assert?5fail?3?5?$CFs?6?$AA@"]
	lea	ecx, ["??_C@_0BD@BKAIAFFE@enqueue?5after?5full?$AA@"]
	mov	dword ptr [esp], eax
	mov	dword ptr [esp + 4], ecx
	call	_printf
	mov	dword ptr [ebp - 4], -1
	mov	dword ptr [ebp - 88], eax # 4-byte Spill
	jmp	LBB4_45
LBB4_16:
	mov	dword ptr [ebp - 40], 0
LBB4_17:                                # =>This Inner Loop Header: Depth=1
	cmp	dword ptr [ebp - 40], 4
	jge	LBB4_24
# BB#18:                                #   in Loop: Header=BB4_17 Depth=1
	lea	eax, [ebp - 44]
	mov	ecx, dword ptr [ebp - 32]
	mov	dword ptr [esp], ecx
	mov	dword ptr [esp + 4], eax
	call	_queue_dequeue
	cmp	eax, 0
	je	LBB4_20
# BB#19:
	lea	eax, ["??_C@_0BB@BLBNCOHH@assert?5fail?3?5?$CFs?6?$AA@"]
	lea	ecx, ["??_C@_05BFLJBJIN@deque?$AA@"]
	mov	dword ptr [esp], eax
	mov	dword ptr [esp + 4], ecx
	call	_printf
	mov	dword ptr [ebp - 4], -1
	mov	dword ptr [ebp - 92], eax # 4-byte Spill
	jmp	LBB4_45
LBB4_20:                                #   in Loop: Header=BB4_17 Depth=1
	mov	eax, dword ptr [ebp - 44]
	mov	ecx, dword ptr [ebp - 40]
	cmp	eax, dword ptr [ebp + 4*ecx - 28]
	je	LBB4_22
# BB#21:
	lea	eax, ["??_C@_0BB@BLBNCOHH@assert?5fail?3?5?$CFs?6?$AA@"]
	lea	ecx, ["??_C@_09FCECJJGJ@pop_value?$AA@"]
	mov	dword ptr [esp], eax
	mov	dword ptr [esp + 4], ecx
	call	_printf
	mov	dword ptr [ebp - 4], -1
	mov	dword ptr [ebp - 96], eax # 4-byte Spill
	jmp	LBB4_45
LBB4_22:                                #   in Loop: Header=BB4_17 Depth=1
	jmp	LBB4_23
LBB4_23:                                #   in Loop: Header=BB4_17 Depth=1
	mov	eax, dword ptr [ebp - 40]
	add	eax, 1
	mov	dword ptr [ebp - 40], eax
	jmp	LBB4_17
LBB4_24:
	mov	eax, dword ptr [ebp - 32]
	mov	dword ptr [esp], eax
	call	_queue_is_empty
	cmp	eax, 0
	jne	LBB4_26
# BB#25:
	lea	eax, ["??_C@_0BB@BLBNCOHH@assert?5fail?3?5?$CFs?6?$AA@"]
	lea	ecx, ["??_C@_06KEKIIPON@empty2?$AA@"]
	mov	dword ptr [esp], eax
	mov	dword ptr [esp + 4], ecx
	call	_printf
	mov	dword ptr [ebp - 4], -1
	mov	dword ptr [ebp - 100], eax # 4-byte Spill
	jmp	LBB4_45
LBB4_26:
	mov	dword ptr [ebp - 48], 0
LBB4_27:                                # =>This Inner Loop Header: Depth=1
	cmp	dword ptr [ebp - 48], 4
	jge	LBB4_32
# BB#28:                                #   in Loop: Header=BB4_27 Depth=1
	mov	eax, dword ptr [ebp - 48]
	mov	eax, dword ptr [ebp + 4*eax - 28]
	mov	ecx, dword ptr [ebp - 32]
	mov	dword ptr [esp], ecx
	mov	dword ptr [esp + 4], eax
	call	_queue_enqueue
	cmp	eax, 0
	je	LBB4_30
# BB#29:
	lea	eax, ["??_C@_0BB@BLBNCOHH@assert?5fail?3?5?$CFs?6?$AA@"]
	lea	ecx, ["??_C@_08HFMMCPPH@enqueue2?$AA@"]
	mov	dword ptr [esp], eax
	mov	dword ptr [esp + 4], ecx
	call	_printf
	mov	dword ptr [ebp - 4], -1
	mov	dword ptr [ebp - 104], eax # 4-byte Spill
	jmp	LBB4_45
LBB4_30:                                #   in Loop: Header=BB4_27 Depth=1
	jmp	LBB4_31
LBB4_31:                                #   in Loop: Header=BB4_27 Depth=1
	mov	eax, dword ptr [ebp - 48]
	add	eax, 1
	mov	dword ptr [ebp - 48], eax
	jmp	LBB4_27
LBB4_32:
	mov	eax, 1
	mov	ecx, dword ptr [ebp - 32]
	mov	dword ptr [esp], ecx
	mov	dword ptr [esp + 4], 1
	mov	dword ptr [ebp - 108], eax # 4-byte Spill
	call	_queue_enqueue
	cmp	eax, -1
	je	LBB4_34
# BB#33:
	lea	eax, ["??_C@_0BB@BLBNCOHH@assert?5fail?3?5?$CFs?6?$AA@"]
	lea	ecx, ["??_C@_0BD@BKAIAFFE@enqueue?5after?5full?$AA@"]
	mov	dword ptr [esp], eax
	mov	dword ptr [esp + 4], ecx
	call	_printf
	mov	dword ptr [ebp - 4], -1
	mov	dword ptr [ebp - 112], eax # 4-byte Spill
	jmp	LBB4_45
LBB4_34:
	mov	dword ptr [ebp - 52], 0
LBB4_35:                                # =>This Inner Loop Header: Depth=1
	cmp	dword ptr [ebp - 52], 4
	jge	LBB4_42
# BB#36:                                #   in Loop: Header=BB4_35 Depth=1
	lea	eax, [ebp - 56]
	mov	ecx, dword ptr [ebp - 32]
	mov	dword ptr [esp], ecx
	mov	dword ptr [esp + 4], eax
	call	_queue_dequeue
	cmp	eax, 0
	je	LBB4_38
# BB#37:
	lea	eax, ["??_C@_0BB@BLBNCOHH@assert?5fail?3?5?$CFs?6?$AA@"]
	lea	ecx, ["??_C@_05BFLJBJIN@deque?$AA@"]
	mov	dword ptr [esp], eax
	mov	dword ptr [esp + 4], ecx
	call	_printf
	mov	dword ptr [ebp - 4], -1
	mov	dword ptr [ebp - 116], eax # 4-byte Spill
	jmp	LBB4_45
LBB4_38:                                #   in Loop: Header=BB4_35 Depth=1
	mov	eax, dword ptr [ebp - 56]
	mov	ecx, dword ptr [ebp - 52]
	cmp	eax, dword ptr [ebp + 4*ecx - 28]
	je	LBB4_40
# BB#39:
	lea	eax, ["??_C@_0BB@BLBNCOHH@assert?5fail?3?5?$CFs?6?$AA@"]
	lea	ecx, ["??_C@_09FCECJJGJ@pop_value?$AA@"]
	mov	dword ptr [esp], eax
	mov	dword ptr [esp + 4], ecx
	call	_printf
	mov	dword ptr [ebp - 4], -1
	mov	dword ptr [ebp - 120], eax # 4-byte Spill
	jmp	LBB4_45
LBB4_40:                                #   in Loop: Header=BB4_35 Depth=1
	jmp	LBB4_41
LBB4_41:                                #   in Loop: Header=BB4_35 Depth=1
	mov	eax, dword ptr [ebp - 52]
	add	eax, 1
	mov	dword ptr [ebp - 52], eax
	jmp	LBB4_35
LBB4_42:
	mov	eax, dword ptr [ebp - 32]
	mov	dword ptr [esp], eax
	call	_queue_is_empty
	cmp	eax, 0
	jne	LBB4_44
# BB#43:
	lea	eax, ["??_C@_0BB@BLBNCOHH@assert?5fail?3?5?$CFs?6?$AA@"]
	lea	ecx, ["??_C@_06LNLDLOKM@empty3?$AA@"]
	mov	dword ptr [esp], eax
	mov	dword ptr [esp + 4], ecx
	call	_printf
	mov	dword ptr [ebp - 4], -1
	mov	dword ptr [ebp - 124], eax # 4-byte Spill
	jmp	LBB4_45
LBB4_44:
	mov	eax, dword ptr [ebp - 32]
	mov	dword ptr [esp], eax
	call	_queue_free
	lea	eax, ["??_C@_0BF@DOCNPEKK@everything?5is?5fine?$CB?6?$AA@"]
	mov	dword ptr [esp], eax
	call	_printf
	mov	dword ptr [ebp - 4], 0
	mov	dword ptr [ebp - 128], eax # 4-byte Spill
LBB4_45:
	mov	eax, dword ptr [ebp - 4]
	add	esp, 136
	pop	ebp
	ret

	.def	 _printf;
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,_printf
	.globl	_printf
	.p2align	4, 0x90
_printf:                                # @printf
# BB#0:
	push	ebp
	mov	ebp, esp
	push	esi
	sub	esp, 36
	mov	eax, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 8], eax
	lea	eax, [ebp + 12]
	mov	dword ptr [ebp - 16], eax
	mov	ecx, dword ptr [ebp - 8]
	mov	edx, esp
	mov	dword ptr [edx], 1
	mov	dword ptr [ebp - 20], eax # 4-byte Spill
	mov	dword ptr [ebp - 24], ecx # 4-byte Spill
	call	___acrt_iob_func
	mov	ecx, esp
	mov	edx, dword ptr [ebp - 20] # 4-byte Reload
	mov	dword ptr [ecx + 12], edx
	mov	esi, dword ptr [ebp - 24] # 4-byte Reload
	mov	dword ptr [ecx + 4], esi
	mov	dword ptr [ecx], eax
	mov	dword ptr [ecx + 8], 0
	call	__vfprintf_l
	mov	dword ptr [ebp - 12], eax
	add	esp, 36
	pop	esi
	pop	ebp
	ret

	.def	 __vsprintf_l;
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,__vsprintf_l
	.globl	__vsprintf_l
	.p2align	4, 0x90
__vsprintf_l:                           # @_vsprintf_l
# BB#0:
	push	ebp
	mov	ebp, esp
	push	edi
	push	esi
	sub	esp, 40
	mov	eax, dword ptr [ebp + 20]
	mov	ecx, dword ptr [ebp + 16]
	mov	edx, dword ptr [ebp + 12]
	mov	esi, dword ptr [ebp + 8]
	mov	edi, 4294967295
	mov	dword ptr [ebp - 12], eax
	mov	dword ptr [ebp - 16], ecx
	mov	dword ptr [ebp - 20], edx
	mov	dword ptr [ebp - 24], esi
	mov	eax, dword ptr [ebp - 12]
	mov	ecx, dword ptr [ebp - 16]
	mov	edx, dword ptr [ebp - 20]
	mov	esi, dword ptr [ebp - 24]
	mov	dword ptr [esp], esi
	mov	dword ptr [esp + 4], -1
	mov	dword ptr [esp + 8], edx
	mov	dword ptr [esp + 12], ecx
	mov	dword ptr [esp + 16], eax
	mov	dword ptr [ebp - 28], edi # 4-byte Spill
	call	__vsnprintf_l
	add	esp, 40
	pop	esi
	pop	edi
	pop	ebp
	ret

	.def	 __vsnprintf_l;
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,__vsnprintf_l
	.globl	__vsnprintf_l
	.p2align	4, 0x90
__vsnprintf_l:                          # @_vsnprintf_l
# BB#0:
	push	ebp
	mov	ebp, esp
	push	ebx
	push	edi
	push	esi
	sub	esp, 76
	mov	eax, dword ptr [ebp + 24]
	mov	ecx, dword ptr [ebp + 20]
	mov	edx, dword ptr [ebp + 16]
	mov	esi, dword ptr [ebp + 12]
	mov	edi, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 16], eax
	mov	dword ptr [ebp - 20], ecx
	mov	dword ptr [ebp - 24], edx
	mov	dword ptr [ebp - 28], esi
	mov	dword ptr [ebp - 32], edi
	mov	eax, dword ptr [ebp - 16]
	mov	ecx, dword ptr [ebp - 20]
	mov	edx, dword ptr [ebp - 24]
	mov	esi, dword ptr [ebp - 28]
	mov	dword ptr [ebp - 40], esi # 4-byte Spill
	mov	dword ptr [ebp - 44], eax # 4-byte Spill
	mov	dword ptr [ebp - 48], ecx # 4-byte Spill
	mov	dword ptr [ebp - 52], edx # 4-byte Spill
	mov	dword ptr [ebp - 56], edi # 4-byte Spill
	call	___local_stdio_printf_options
	mov	ecx, dword ptr [eax]
	mov	eax, dword ptr [eax + 4]
	or	ecx, 1
	mov	edx, esp
	mov	esi, dword ptr [ebp - 44] # 4-byte Reload
	mov	dword ptr [edx + 24], esi
	mov	edi, dword ptr [ebp - 48] # 4-byte Reload
	mov	dword ptr [edx + 20], edi
	mov	ebx, dword ptr [ebp - 52] # 4-byte Reload
	mov	dword ptr [edx + 16], ebx
	mov	esi, dword ptr [ebp - 40] # 4-byte Reload
	mov	dword ptr [edx + 12], esi
	mov	esi, dword ptr [ebp - 56] # 4-byte Reload
	mov	dword ptr [edx + 8], esi
	mov	dword ptr [edx + 4], eax
	mov	dword ptr [edx], ecx
	call	___stdio_common_vsprintf
	mov	dword ptr [ebp - 36], eax
	cmp	dword ptr [ebp - 36], 0
	jge	LBB7_2
# BB#1:
	mov	eax, 4294967295
	mov	dword ptr [ebp - 60], eax # 4-byte Spill
	jmp	LBB7_3
LBB7_2:
	mov	eax, dword ptr [ebp - 36]
	mov	dword ptr [ebp - 60], eax # 4-byte Spill
LBB7_3:
	mov	eax, dword ptr [ebp - 60] # 4-byte Reload
	add	esp, 76
	pop	esi
	pop	edi
	pop	ebx
	pop	ebp
	ret

	.def	 ___local_stdio_printf_options;
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,___local_stdio_printf_options
	.globl	___local_stdio_printf_options
	.p2align	4, 0x90
___local_stdio_printf_options:          # @__local_stdio_printf_options
# BB#0:
	push	ebp
	mov	ebp, esp
	lea	eax, [___local_stdio_printf_options._OptionsStorage]
	pop	ebp
	ret

	.def	 __vfprintf_l;
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,__vfprintf_l
	.globl	__vfprintf_l
	.p2align	4, 0x90
__vfprintf_l:                           # @_vfprintf_l
# BB#0:
	push	ebp
	mov	ebp, esp
	push	ebx
	push	edi
	push	esi
	sub	esp, 56
	mov	eax, dword ptr [ebp + 20]
	mov	ecx, dword ptr [ebp + 16]
	mov	edx, dword ptr [ebp + 12]
	mov	esi, dword ptr [ebp + 8]
	mov	dword ptr [ebp - 16], eax
	mov	dword ptr [ebp - 20], ecx
	mov	dword ptr [ebp - 24], edx
	mov	dword ptr [ebp - 28], esi
	mov	eax, dword ptr [ebp - 16]
	mov	ecx, dword ptr [ebp - 20]
	mov	edx, dword ptr [ebp - 24]
	mov	dword ptr [ebp - 32], edx # 4-byte Spill
	mov	dword ptr [ebp - 36], eax # 4-byte Spill
	mov	dword ptr [ebp - 40], ecx # 4-byte Spill
	mov	dword ptr [ebp - 44], esi # 4-byte Spill
	call	___local_stdio_printf_options
	mov	ecx, dword ptr [eax]
	mov	eax, dword ptr [eax + 4]
	mov	edx, esp
	mov	esi, dword ptr [ebp - 36] # 4-byte Reload
	mov	dword ptr [edx + 20], esi
	mov	edi, dword ptr [ebp - 40] # 4-byte Reload
	mov	dword ptr [edx + 16], edi
	mov	ebx, dword ptr [ebp - 32] # 4-byte Reload
	mov	dword ptr [edx + 12], ebx
	mov	esi, dword ptr [ebp - 44] # 4-byte Reload
	mov	dword ptr [edx + 8], esi
	mov	dword ptr [edx + 4], eax
	mov	dword ptr [edx], ecx
	call	___stdio_common_vfprintf
	add	esp, 56
	pop	esi
	pop	edi
	pop	ebx
	pop	ebp
	ret

	.section	.rdata,"dr"
	.p2align	2               # @main.a
L_main.a:
	.long	5                       # 0x5
	.long	4                       # 0x4
	.long	5                       # 0x5
	.long	3                       # 0x3

	.section	.rdata,"dr",discard,"??_C@_04HKFDNBLD@init?$AA@"
	.globl	"??_C@_04HKFDNBLD@init?$AA@" # @"\01??_C@_04HKFDNBLD@init?$AA@"
"??_C@_04HKFDNBLD@init?$AA@":
	.asciz	"init"

	.section	.rdata,"dr",discard,"??_C@_0BB@BLBNCOHH@assert?5fail?3?5?$CFs?6?$AA@"
	.globl	"??_C@_0BB@BLBNCOHH@assert?5fail?3?5?$CFs?6?$AA@" # @"\01??_C@_0BB@BLBNCOHH@assert?5fail?3?5?$CFs?6?$AA@"
"??_C@_0BB@BLBNCOHH@assert?5fail?3?5?$CFs?6?$AA@":
	.asciz	"assert fail: %s\n"

	.section	.rdata,"dr",discard,"??_C@_05LBJMNBOG@empty?$AA@"
	.globl	"??_C@_05LBJMNBOG@empty?$AA@" # @"\01??_C@_05LBJMNBOG@empty?$AA@"
"??_C@_05LBJMNBOG@empty?$AA@":
	.asciz	"empty"

	.section	.rdata,"dr",discard,"??_C@_04IAGNFIBA@size?$AA@"
	.globl	"??_C@_04IAGNFIBA@size?$AA@" # @"\01??_C@_04IAGNFIBA@size?$AA@"
"??_C@_04IAGNFIBA@size?$AA@":
	.asciz	"size"

	.section	.rdata,"dr",discard,"??_C@_07GOFLDKEC@enqueue?$AA@"
	.globl	"??_C@_07GOFLDKEC@enqueue?$AA@" # @"\01??_C@_07GOFLDKEC@enqueue?$AA@"
"??_C@_07GOFLDKEC@enqueue?$AA@":
	.asciz	"enqueue"

	.section	.rdata,"dr",discard,"??_C@_04PLMLMMEO@full?$AA@"
	.globl	"??_C@_04PLMLMMEO@full?$AA@" # @"\01??_C@_04PLMLMMEO@full?$AA@"
"??_C@_04PLMLMMEO@full?$AA@":
	.asciz	"full"

	.section	.rdata,"dr",discard,"??_C@_0BD@BKAIAFFE@enqueue?5after?5full?$AA@"
	.globl	"??_C@_0BD@BKAIAFFE@enqueue?5after?5full?$AA@" # @"\01??_C@_0BD@BKAIAFFE@enqueue?5after?5full?$AA@"
"??_C@_0BD@BKAIAFFE@enqueue?5after?5full?$AA@":
	.asciz	"enqueue after full"

	.section	.rdata,"dr",discard,"??_C@_05BFLJBJIN@deque?$AA@"
	.globl	"??_C@_05BFLJBJIN@deque?$AA@" # @"\01??_C@_05BFLJBJIN@deque?$AA@"
"??_C@_05BFLJBJIN@deque?$AA@":
	.asciz	"deque"

	.section	.rdata,"dr",discard,"??_C@_09FCECJJGJ@pop_value?$AA@"
	.globl	"??_C@_09FCECJJGJ@pop_value?$AA@" # @"\01??_C@_09FCECJJGJ@pop_value?$AA@"
"??_C@_09FCECJJGJ@pop_value?$AA@":
	.asciz	"pop_value"

	.section	.rdata,"dr",discard,"??_C@_06KEKIIPON@empty2?$AA@"
	.globl	"??_C@_06KEKIIPON@empty2?$AA@" # @"\01??_C@_06KEKIIPON@empty2?$AA@"
"??_C@_06KEKIIPON@empty2?$AA@":
	.asciz	"empty2"

	.section	.rdata,"dr",discard,"??_C@_08HFMMCPPH@enqueue2?$AA@"
	.globl	"??_C@_08HFMMCPPH@enqueue2?$AA@" # @"\01??_C@_08HFMMCPPH@enqueue2?$AA@"
"??_C@_08HFMMCPPH@enqueue2?$AA@":
	.asciz	"enqueue2"

	.section	.rdata,"dr",discard,"??_C@_06LNLDLOKM@empty3?$AA@"
	.globl	"??_C@_06LNLDLOKM@empty3?$AA@" # @"\01??_C@_06LNLDLOKM@empty3?$AA@"
"??_C@_06LNLDLOKM@empty3?$AA@":
	.asciz	"empty3"

	.section	.rdata,"dr",discard,"??_C@_0BF@DOCNPEKK@everything?5is?5fine?$CB?6?$AA@"
	.globl	"??_C@_0BF@DOCNPEKK@everything?5is?5fine?$CB?6?$AA@" # @"\01??_C@_0BF@DOCNPEKK@everything?5is?5fine?$CB?6?$AA@"
"??_C@_0BF@DOCNPEKK@everything?5is?5fine?$CB?6?$AA@":
	.asciz	"everything is fine!\n"

	.lcomm	___local_stdio_printf_options._OptionsStorage,8,8 # @__local_stdio_printf_options._OptionsStorage

