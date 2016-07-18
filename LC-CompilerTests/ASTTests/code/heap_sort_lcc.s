	.data
	.bss
	.text
	.intel_syntax noprefix
	# Frame Size: 0
	# EBP        UID   SYMBOL     TYPE                
	# 8          0     i          int                 
	.globl _parent
_parent:
	push   ebp
	mov    ebp, esp
	sub    esp, 0
	# return / ((int)(i)) (2)
	# / ((int)(i)) (2)
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# 2
	mov    eax, 2
	mov    ebx, eax
	pop    eax
	cdq
	idiv   ebx
	jmp    __parent_return
__parent_return:
	add    esp, 0
	pop    ebp
	ret
	# Frame Size: 0
	# EBP        UID   SYMBOL     TYPE                
	# 8          0     i          int                 
	.globl _left
_left:
	push   ebp
	mov    ebp, esp
	sub    esp, 0
	# return * (2) ((int)(i))
	# * (2) ((int)(i))
	# 2
	mov    eax, 2
	push   eax
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp + 8]
	mov    ebx, dword ptr [eax + 0]
	pop    eax
	imul   eax, ebx
	jmp    __left_return
__left_return:
	add    esp, 0
	pop    ebp
	ret
	# Frame Size: 0
	# EBP        UID   SYMBOL     TYPE                
	# 8          0     i          int                 
	.globl _right
_right:
	push   ebp
	mov    ebp, esp
	sub    esp, 0
	# return + (* (2) ((int)(i))) (1)
	# + (* (2) ((int)(i))) (1)
	# * (2) ((int)(i))
	# 2
	mov    eax, 2
	push   eax
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp + 8]
	mov    ebx, dword ptr [eax + 0]
	pop    eax
	imul   eax, ebx
	push   eax
	# 1
	mov    eax, 1
	mov    ebx, eax
	pop    eax
	add    eax, ebx
	jmp    __right_return
__right_return:
	add    esp, 0
	pop    ebp
	ret
	# Frame Size: 4
	# EBP        UID   SYMBOL     TYPE                
	# 16         2     j          int                 
	# 12         1     i          int                 
	# 8          0     arr        (int) *             
	# -4         3     t          int                 
	.globl _swap
_swap:
	push   ebp
	mov    ebp, esp
	sub    esp, 4
	# t = (int)((((int) *)(arr))[i])
	# (int)((((int) *)(arr))[i])
	# (((int) *)(arr))[i]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# i
	lea    eax, dword ptr [ebp + 12]
	imul   ebx, dword ptr [eax + 0], 4
	pop    eax
	add    eax, ebx
	push   eax
	# t
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, eax
	pop    eax
	mov    eax, dword ptr [eax + 0]
	mov    dword ptr [ebx + 0], eax
	# (((int) *)(arr))[i] = (int)((((int) *)(arr))[j])
	# (int)((((int) *)(arr))[j])
	# (((int) *)(arr))[j]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# j
	lea    eax, dword ptr [ebp + 16]
	imul   ebx, dword ptr [eax + 0], 4
	pop    eax
	add    eax, ebx
	push   eax
	# (((int) *)(arr))[i]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# i
	lea    eax, dword ptr [ebp + 12]
	imul   ebx, dword ptr [eax + 0], 4
	pop    eax
	add    eax, ebx
	mov    ebx, eax
	pop    eax
	mov    eax, dword ptr [eax + 0]
	mov    dword ptr [ebx + 0], eax
	# (((int) *)(arr))[j] = (int)(t)
	# (int)(t)
	# t
	lea    eax, dword ptr [ebp - 4]
	push   eax
	# (((int) *)(arr))[j]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# j
	lea    eax, dword ptr [ebp + 16]
	imul   ebx, dword ptr [eax + 0], 4
	pop    eax
	add    eax, ebx
	mov    ebx, eax
	pop    eax
	mov    eax, dword ptr [eax + 0]
	mov    dword ptr [ebx + 0], eax
__swap_return:
	add    esp, 4
	pop    ebp
	ret
	# Frame Size: 12
	# EBP        UID   SYMBOL     TYPE                
	# 16         2     i          int                 
	# 12         1     len        int                 
	# 8          0     arr        (int) *             
	# -4         3     lhs        int                 
	# -8         4     rhs        int                 
	# -12        5     maximum    int                 
	.globl _max_heapify
_max_heapify:
	push   ebp
	mov    ebp, esp
	sub    esp, 12
	# lhs = (((int) -> int) *)(left)((int)(i))
	# (((int) -> int) *)(left)((int)(i))
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp + 16]
	push   dword ptr [eax + 0]
	# (((int) -> int) *)(left)
	# left
	lea    eax, dword ptr [_left + 0]
	call   eax
	add    esp, 4
	push   eax
	# lhs
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# rhs = (((int) -> int) *)(right)((int)(i))
	# (((int) -> int) *)(right)((int)(i))
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp + 16]
	push   dword ptr [eax + 0]
	# (((int) -> int) *)(right)
	# right
	lea    eax, dword ptr [_right + 0]
	call   eax
	add    esp, 4
	push   eax
	# rhs
	lea    eax, dword ptr [ebp - 8]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# if
	# && (< ((int)(lhs)) ((int)(len))) (> ((int)((((int) *)(arr))[lhs])) ((int)((((int) *)(arr))[i])))
	# < ((int)(lhs)) ((int)(len))
	# (int)(lhs)
	# lhs
	lea    eax, dword ptr [ebp - 4]
	push   dword ptr [eax + 0]
	# (int)(len)
	# len
	lea    eax, dword ptr [ebp + 12]
	mov    ebx, dword ptr [eax + 0]
	pop    eax
	cmp    eax, ebx
	setl   al
	and    al, 1
	movzx  eax, al
	mov    eax, eax
	cmp    eax, 0
	je     __logical_shortcut_0
	# > ((int)((((int) *)(arr))[lhs])) ((int)((((int) *)(arr))[i]))
	# (int)((((int) *)(arr))[lhs])
	# (((int) *)(arr))[lhs]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# lhs
	lea    eax, dword ptr [ebp - 4]
	imul   ebx, dword ptr [eax + 0], 4
	pop    eax
	add    eax, ebx
	push   dword ptr [eax + 0]
	# (int)((((int) *)(arr))[i])
	# (((int) *)(arr))[i]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# i
	lea    eax, dword ptr [ebp + 16]
	imul   ebx, dword ptr [eax + 0], 4
	pop    eax
	add    eax, ebx
	mov    ebx, dword ptr [eax + 0]
	pop    eax
	cmp    eax, ebx
	setg   al
	and    al, 1
	movzx  eax, al
	mov    eax, eax
	cmp    eax, 0
	je     __logical_shortcut_0
	mov    eax, 1
	jmp    __logical_end_0
__logical_shortcut_0:
	mov    eax, 0
__logical_end_0:
	mov    eax, eax
	cmp    eax, 0
	je     __else_block_0
	# then
	# maximum = (int)(lhs)
	# (int)(lhs)
	# lhs
	lea    eax, dword ptr [ebp - 4]
	push   eax
	# maximum
	lea    eax, dword ptr [ebp - 12]
	mov    ebx, eax
	pop    eax
	mov    eax, dword ptr [eax + 0]
	mov    dword ptr [ebx + 0], eax
	jmp    __endif_0
	# else
__else_block_0:
	# maximum = (int)(i)
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp + 16]
	push   eax
	# maximum
	lea    eax, dword ptr [ebp - 12]
	mov    ebx, eax
	pop    eax
	mov    eax, dword ptr [eax + 0]
	mov    dword ptr [ebx + 0], eax
__endif_0:
	# if
	# && (< ((int)(rhs)) ((int)(len))) (> ((int)((((int) *)(arr))[rhs])) ((int)((((int) *)(arr))[maximum])))
	# < ((int)(rhs)) ((int)(len))
	# (int)(rhs)
	# rhs
	lea    eax, dword ptr [ebp - 8]
	push   dword ptr [eax + 0]
	# (int)(len)
	# len
	lea    eax, dword ptr [ebp + 12]
	mov    ebx, dword ptr [eax + 0]
	pop    eax
	cmp    eax, ebx
	setl   al
	and    al, 1
	movzx  eax, al
	mov    eax, eax
	cmp    eax, 0
	je     __logical_shortcut_1
	# > ((int)((((int) *)(arr))[rhs])) ((int)((((int) *)(arr))[maximum]))
	# (int)((((int) *)(arr))[rhs])
	# (((int) *)(arr))[rhs]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# rhs
	lea    eax, dword ptr [ebp - 8]
	imul   ebx, dword ptr [eax + 0], 4
	pop    eax
	add    eax, ebx
	push   dword ptr [eax + 0]
	# (int)((((int) *)(arr))[maximum])
	# (((int) *)(arr))[maximum]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# maximum
	lea    eax, dword ptr [ebp - 12]
	imul   ebx, dword ptr [eax + 0], 4
	pop    eax
	add    eax, ebx
	mov    ebx, dword ptr [eax + 0]
	pop    eax
	cmp    eax, ebx
	setg   al
	and    al, 1
	movzx  eax, al
	mov    eax, eax
	cmp    eax, 0
	je     __logical_shortcut_1
	mov    eax, 1
	jmp    __logical_end_1
__logical_shortcut_1:
	mov    eax, 0
__logical_end_1:
	mov    eax, eax
	cmp    eax, 0
	je     __endif_1
	# then
	# maximum = (int)(rhs)
	# (int)(rhs)
	# rhs
	lea    eax, dword ptr [ebp - 8]
	push   eax
	# maximum
	lea    eax, dword ptr [ebp - 12]
	mov    ebx, eax
	pop    eax
	mov    eax, dword ptr [eax + 0]
	mov    dword ptr [ebx + 0], eax
	jmp    __endif_1
__endif_1:
	# if
	# != ((int)(maximum)) ((int)(i))
	# (int)(maximum)
	# maximum
	lea    eax, dword ptr [ebp - 12]
	push   dword ptr [eax + 0]
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp + 16]
	mov    ebx, dword ptr [eax + 0]
	pop    eax
	cmp    eax, ebx
	setne  al
	and    al, 1
	movzx  eax, al
	mov    eax, eax
	cmp    eax, 0
	je     __endif_2
	# then
	# ((((int) *, int, int) -> void) *)(swap)(((int) *)(arr), (int)(i), (int)(maximum))
	# (int)(maximum)
	# maximum
	lea    eax, dword ptr [ebp - 12]
	push   dword ptr [eax + 0]
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp + 16]
	push   dword ptr [eax + 0]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# ((((int) *, int, int) -> void) *)(swap)
	# swap
	lea    eax, dword ptr [_swap + 0]
	call   eax
	add    esp, 12
	# ((((int) *, int, int) -> void) *)(max_heapify)(((int) *)(arr), (int)(len), (int)(maximum))
	# (int)(maximum)
	# maximum
	lea    eax, dword ptr [ebp - 12]
	push   dword ptr [eax + 0]
	# (int)(len)
	# len
	lea    eax, dword ptr [ebp + 12]
	push   dword ptr [eax + 0]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# ((((int) *, int, int) -> void) *)(max_heapify)
	# max_heapify
	lea    eax, dword ptr [_max_heapify + 0]
	call   eax
	add    esp, 12
	jmp    __endif_2
__endif_2:
__max_heapify_return:
	add    esp, 12
	pop    ebp
	ret
	# Frame Size: 4
	# EBP        UID   SYMBOL     TYPE                
	# 12         1     len        int                 
	# 8          0     arr        (int) *             
	# -4         2     i          int                 
	.globl _build_max_heap
_build_max_heap:
	push   ebp
	mov    ebp, esp
	sub    esp, 4
	# for init
	# i = - (/ ((int)(len)) (2)) (1)
	# - (/ ((int)(len)) (2)) (1)
	# / ((int)(len)) (2)
	# (int)(len)
	# len
	lea    eax, dword ptr [ebp + 12]
	push   dword ptr [eax + 0]
	# 2
	mov    eax, 2
	mov    ebx, eax
	pop    eax
	cdq
	idiv   ebx
	push   eax
	# 1
	mov    eax, 1
	mov    ebx, eax
	pop    eax
	sub    eax, ebx
	push   eax
	# i
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	jmp    __loop_first_0
	# for iter
__loop_second_plus_0:
	# i--
	# i
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, dword ptr [eax + 0]
	dec    dword ptr [eax + 0]
	mov    eax, ebx
	# for pred
__loop_first_0:
	# >= ((int)(i)) (0)
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp - 4]
	push   dword ptr [eax + 0]
	# 0
	mov    eax, 0
	mov    ebx, eax
	pop    eax
	cmp    eax, ebx
	setge  al
	and    al, 1
	movzx  eax, al
	mov    eax, eax
	cmp    eax, 0
	je     __loop_break_0
	# for body
	# ((((int) *, int, int) -> void) *)(max_heapify)(((int) *)(arr), (int)(len), (int)(i))
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp - 4]
	push   dword ptr [eax + 0]
	# (int)(len)
	# len
	lea    eax, dword ptr [ebp + 12]
	push   dword ptr [eax + 0]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# ((((int) *, int, int) -> void) *)(max_heapify)
	# max_heapify
	lea    eax, dword ptr [_max_heapify + 0]
	call   eax
	add    esp, 12
__loop_continure0:
	jmp    __loop_second_plus_0
	# for end
__loop_break_0:
__build_max_heap_return:
	add    esp, 4
	pop    ebp
	ret
	# Frame Size: 4
	# EBP        UID   SYMBOL     TYPE                
	# 12         1     len        int                 
	# 8          0     arr        (int) *             
	# -4         2     i          int                 
	.globl _heap_sort
_heap_sort:
	push   ebp
	mov    ebp, esp
	sub    esp, 4
	# ((((int) *, int) -> void) *)(build_max_heap)(((int) *)(arr), (int)(len))
	# (int)(len)
	# len
	lea    eax, dword ptr [ebp + 12]
	push   dword ptr [eax + 0]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# ((((int) *, int) -> void) *)(build_max_heap)
	# build_max_heap
	lea    eax, dword ptr [_build_max_heap + 0]
	call   eax
	add    esp, 8
	# for init
	# i = - ((int)(len)) (1)
	# - ((int)(len)) (1)
	# (int)(len)
	# len
	lea    eax, dword ptr [ebp + 12]
	push   dword ptr [eax + 0]
	# 1
	mov    eax, 1
	mov    ebx, eax
	pop    eax
	sub    eax, ebx
	push   eax
	# i
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	jmp    __loop_first_1
	# for iter
__loop_second_plus_1:
	# i--
	# i
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, dword ptr [eax + 0]
	dec    dword ptr [eax + 0]
	mov    eax, ebx
	# for pred
__loop_first_1:
	# >= ((int)(i)) (1)
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp - 4]
	push   dword ptr [eax + 0]
	# 1
	mov    eax, 1
	mov    ebx, eax
	pop    eax
	cmp    eax, ebx
	setge  al
	and    al, 1
	movzx  eax, al
	mov    eax, eax
	cmp    eax, 0
	je     __loop_break_1
	# for body
	# ((((int) *, int, int) -> void) *)(swap)(((int) *)(arr), (int)(i), 0)
	# 0
	mov    eax, 0
	push   eax
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp - 4]
	push   dword ptr [eax + 0]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# ((((int) *, int, int) -> void) *)(swap)
	# swap
	lea    eax, dword ptr [_swap + 0]
	call   eax
	add    esp, 12
	# ((((int) *, int, int) -> void) *)(max_heapify)(((int) *)(arr), (int)(i), 0)
	# 0
	mov    eax, 0
	push   eax
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp - 4]
	push   dword ptr [eax + 0]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# ((((int) *, int, int) -> void) *)(max_heapify)
	# max_heapify
	lea    eax, dword ptr [_max_heapify + 0]
	call   eax
	add    esp, 12
__loop_continure1:
	jmp    __loop_second_plus_1
	# for end
__loop_break_1:
__heap_sort_return:
	add    esp, 4
	pop    ebp
	ret
