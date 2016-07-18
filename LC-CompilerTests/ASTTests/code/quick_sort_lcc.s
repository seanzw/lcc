	.data
	.bss
	.text
	.intel_syntax noprefix
	# Frame Size: 16
	# EBP        UID   SYMBOL     TYPE                
	# 16         2     rhs        int                 
	# 12         1     lhs        int                 
	# 8          0     arr        (int) *             
	# -4         3     x          int                 
	# -8         4     i          int                 
	# -12        5     j          int                 
	# -16        6     t          int                 
	.globl _partition
_partition:
	push   ebp
	mov    ebp, esp
	sub    esp, 16
	# x = (int)((((int) *)(arr))[rhs])
	# (int)((((int) *)(arr))[rhs])
	# (((int) *)(arr))[rhs]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# rhs
	lea    eax, dword ptr [ebp + 16]
	imul   ebx, dword ptr [eax + 0], 4
	pop    eax
	add    eax, ebx
	push   eax
	# x
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, eax
	pop    eax
	mov    eax, dword ptr [eax + 0]
	mov    dword ptr [ebx + 0], eax
	# i = - ((int)(lhs)) (1)
	# - ((int)(lhs)) (1)
	# (int)(lhs)
	# lhs
	lea    eax, dword ptr [ebp + 12]
	push   dword ptr [eax + 0]
	# 1
	mov    eax, 1
	mov    ebx, eax
	pop    eax
	sub    eax, ebx
	push   eax
	# i
	lea    eax, dword ptr [ebp - 8]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# for init
	# j = (int)(lhs)
	# (int)(lhs)
	# lhs
	lea    eax, dword ptr [ebp + 12]
	push   eax
	# j
	lea    eax, dword ptr [ebp - 12]
	mov    ebx, eax
	pop    eax
	mov    eax, dword ptr [eax + 0]
	mov    dword ptr [ebx + 0], eax
	jmp    __loop_first_0
	# for iter
__loop_second_plus_0:
	# j++
	# j
	lea    eax, dword ptr [ebp - 12]
	mov    ebx, dword ptr [eax + 0]
	inc    dword ptr [eax + 0]
	mov    eax, ebx
	# for pred
__loop_first_0:
	# <= ((int)(j)) (- ((int)(rhs)) (1))
	# (int)(j)
	# j
	lea    eax, dword ptr [ebp - 12]
	push   dword ptr [eax + 0]
	# - ((int)(rhs)) (1)
	# (int)(rhs)
	# rhs
	lea    eax, dword ptr [ebp + 16]
	push   dword ptr [eax + 0]
	# 1
	mov    eax, 1
	mov    ebx, eax
	pop    eax
	sub    eax, ebx
	mov    ebx, eax
	pop    eax
	cmp    eax, ebx
	setle  al
	and    al, 1
	movzx  eax, al
	mov    eax, eax
	cmp    eax, 0
	je     __loop_break_0
	# for body
	# if
	# <= ((int)((((int) *)(arr))[j])) ((int)(x))
	# (int)((((int) *)(arr))[j])
	# (((int) *)(arr))[j]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# j
	lea    eax, dword ptr [ebp - 12]
	imul   ebx, dword ptr [eax + 0], 4
	pop    eax
	add    eax, ebx
	push   dword ptr [eax + 0]
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, dword ptr [eax + 0]
	pop    eax
	cmp    eax, ebx
	setle  al
	and    al, 1
	movzx  eax, al
	mov    eax, eax
	cmp    eax, 0
	je     __endif_0
	# then
	# i++
	# i
	lea    eax, dword ptr [ebp - 8]
	mov    ebx, dword ptr [eax + 0]
	inc    dword ptr [eax + 0]
	mov    eax, ebx
	# t = (int)((((int) *)(arr))[i])
	# (int)((((int) *)(arr))[i])
	# (((int) *)(arr))[i]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# i
	lea    eax, dword ptr [ebp - 8]
	imul   ebx, dword ptr [eax + 0], 4
	pop    eax
	add    eax, ebx
	push   eax
	# t
	lea    eax, dword ptr [ebp - 16]
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
	lea    eax, dword ptr [ebp - 12]
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
	lea    eax, dword ptr [ebp - 8]
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
	lea    eax, dword ptr [ebp - 16]
	push   eax
	# (((int) *)(arr))[j]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# j
	lea    eax, dword ptr [ebp - 12]
	imul   ebx, dword ptr [eax + 0], 4
	pop    eax
	add    eax, ebx
	mov    ebx, eax
	pop    eax
	mov    eax, dword ptr [eax + 0]
	mov    dword ptr [ebx + 0], eax
	jmp    __endif_0
__endif_0:
__loop_continure0:
	jmp    __loop_second_plus_0
	# for end
__loop_break_0:
	# t = (int)((((int) *)(arr))[+ ((int)(i)) (1)])
	# (int)((((int) *)(arr))[+ ((int)(i)) (1)])
	# (((int) *)(arr))[+ ((int)(i)) (1)]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# + ((int)(i)) (1)
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp - 8]
	push   dword ptr [eax + 0]
	# 1
	mov    eax, 1
	mov    ebx, eax
	pop    eax
	add    eax, ebx
	imul   ebx, eax, 4
	pop    eax
	add    eax, ebx
	push   eax
	# t
	lea    eax, dword ptr [ebp - 16]
	mov    ebx, eax
	pop    eax
	mov    eax, dword ptr [eax + 0]
	mov    dword ptr [ebx + 0], eax
	# (((int) *)(arr))[+ ((int)(i)) (1)] = (int)((((int) *)(arr))[rhs])
	# (int)((((int) *)(arr))[rhs])
	# (((int) *)(arr))[rhs]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# rhs
	lea    eax, dword ptr [ebp + 16]
	imul   ebx, dword ptr [eax + 0], 4
	pop    eax
	add    eax, ebx
	push   eax
	# (((int) *)(arr))[+ ((int)(i)) (1)]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# + ((int)(i)) (1)
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp - 8]
	push   dword ptr [eax + 0]
	# 1
	mov    eax, 1
	mov    ebx, eax
	pop    eax
	add    eax, ebx
	imul   ebx, eax, 4
	pop    eax
	add    eax, ebx
	mov    ebx, eax
	pop    eax
	mov    eax, dword ptr [eax + 0]
	mov    dword ptr [ebx + 0], eax
	# (((int) *)(arr))[rhs] = (int)(t)
	# (int)(t)
	# t
	lea    eax, dword ptr [ebp - 16]
	push   eax
	# (((int) *)(arr))[rhs]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# rhs
	lea    eax, dword ptr [ebp + 16]
	imul   ebx, dword ptr [eax + 0], 4
	pop    eax
	add    eax, ebx
	mov    ebx, eax
	pop    eax
	mov    eax, dword ptr [eax + 0]
	mov    dword ptr [ebx + 0], eax
	# return + ((int)(i)) (1)
	# + ((int)(i)) (1)
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp - 8]
	push   dword ptr [eax + 0]
	# 1
	mov    eax, 1
	mov    ebx, eax
	pop    eax
	add    eax, ebx
	jmp    __partition_return
__partition_return:
	add    esp, 16
	pop    ebp
	ret
	# Frame Size: 4
	# EBP        UID   SYMBOL     TYPE                
	# 16         2     rhs        int                 
	# 12         1     lhs        int                 
	# 8          0     arr        (int) *             
	# -4         3     mid        int                 
	.globl _quick_sort
_quick_sort:
	push   ebp
	mov    ebp, esp
	sub    esp, 4
	# if
	# < ((int)(lhs)) ((int)(rhs))
	# (int)(lhs)
	# lhs
	lea    eax, dword ptr [ebp + 12]
	push   dword ptr [eax + 0]
	# (int)(rhs)
	# rhs
	lea    eax, dword ptr [ebp + 16]
	mov    ebx, dword ptr [eax + 0]
	pop    eax
	cmp    eax, ebx
	setl   al
	and    al, 1
	movzx  eax, al
	mov    eax, eax
	cmp    eax, 0
	je     __endif_1
	# then
	# mid = ((((int) *, int, int) -> int) *)(partition)(((int) *)(arr), (int)(lhs), (int)(rhs))
	# ((((int) *, int, int) -> int) *)(partition)(((int) *)(arr), (int)(lhs), (int)(rhs))
	# (int)(rhs)
	# rhs
	lea    eax, dword ptr [ebp + 16]
	push   dword ptr [eax + 0]
	# (int)(lhs)
	# lhs
	lea    eax, dword ptr [ebp + 12]
	push   dword ptr [eax + 0]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# ((((int) *, int, int) -> int) *)(partition)
	# partition
	lea    eax, dword ptr [_partition + 0]
	call   eax
	add    esp, 12
	push   eax
	# mid
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# ((((int) *, int, int) -> void) *)(quick_sort)(((int) *)(arr), (int)(lhs), - ((int)(mid)) (1))
	# - ((int)(mid)) (1)
	# (int)(mid)
	# mid
	lea    eax, dword ptr [ebp - 4]
	push   dword ptr [eax + 0]
	# 1
	mov    eax, 1
	mov    ebx, eax
	pop    eax
	sub    eax, ebx
	push   eax
	# (int)(lhs)
	# lhs
	lea    eax, dword ptr [ebp + 12]
	push   dword ptr [eax + 0]
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# ((((int) *, int, int) -> void) *)(quick_sort)
	# quick_sort
	lea    eax, dword ptr [_quick_sort + 0]
	call   eax
	add    esp, 12
	# ((((int) *, int, int) -> void) *)(quick_sort)(((int) *)(arr), + ((int)(mid)) (1), (int)(rhs))
	# (int)(rhs)
	# rhs
	lea    eax, dword ptr [ebp + 16]
	push   dword ptr [eax + 0]
	# + ((int)(mid)) (1)
	# (int)(mid)
	# mid
	lea    eax, dword ptr [ebp - 4]
	push   dword ptr [eax + 0]
	# 1
	mov    eax, 1
	mov    ebx, eax
	pop    eax
	add    eax, ebx
	push   eax
	# ((int) *)(arr)
	# arr
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# ((((int) *, int, int) -> void) *)(quick_sort)
	# quick_sort
	lea    eax, dword ptr [_quick_sort + 0]
	call   eax
	add    esp, 12
	jmp    __endif_1
__endif_1:
__quick_sort_return:
	add    esp, 4
	pop    ebp
	ret
