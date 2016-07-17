	.data
	.bss
	.text
	.intel_syntax noprefix
	# Frame Size: 0
	# EBP        UID   SYMBOL     TYPE                
	# 8          0     x          int                 
	.globl _test_if
_test_if:
	push   ebp
	mov    ebp, esp
	sub    esp, 0
	# if
	# (int)(x) <= 0
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	push   eax
	# 0
	mov    eax, 0
	mov    ebx, eax
	pop    eax
	cmp    eax, ebx
	setle  al
	and    al, 1
	movzx  eax, al
	cmp    eax, 0
	je     __else_block_1
	# then
	# if
	# (int)(x) <= 2
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	push   eax
	# 2
	mov    eax, 2
	mov    ebx, eax
	pop    eax
	cmp    eax, ebx
	setle  al
	and    al, 1
	movzx  eax, al
	cmp    eax, 0
	je     __endif_0
	# then
	# return (int)(x) + 1
	# (int)(x) + 1
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	push   eax
	# 1
	mov    eax, 1
	mov    ebx, eax
	pop    eax
	add    eax, ebx
	jmp    __test_if_return
	jmp    __endif_0
__endif_0:
	# return x
	# x
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	jmp    __test_if_return
	jmp    __endif_1
	# else
__else_block_1:
	# return (int)(x) - 4
	# (int)(x) - 4
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	push   eax
	# 4
	mov    eax, 4
	mov    ebx, eax
	pop    eax
	sub    eax, ebx
	jmp    __test_if_return
__endif_1:
__test_if_return:
	add    esp, 0
	pop    ebp
	ret
	# Frame Size: 8
	# EBP        UID   SYMBOL     TYPE                
	# 8          0     x          int                 
	# -4         1     i          int                 
	# -8         2     sum        int                 
	.globl _test_for
_test_for:
	push   ebp
	mov    ebp, esp
	sub    esp, 8
	# sum = 0
	# 0
	mov    eax, 0
	push   eax
	# sum
	lea    eax, dword ptr [ebp - 8]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# for init
	# i = 0
	# 0
	mov    eax, 0
	push   eax
	# i
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	jmp    __loop_first_0
	# for iter
__loop_second_plus_0:
	# i++
	# i
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, dword ptr [eax + 0]
	inc    dword ptr [eax + 0]
	mov    eax, ebx
	# for pred
__loop_first_0:
	# (int)(i) < (int)(x)
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp - 4]
	mov    eax, dword ptr [eax + 0]
	push   eax
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	mov    ebx, dword ptr [eax + 0]
	pop    eax
	cmp    eax, ebx
	setl   al
	and    al, 1
	movzx  eax, al
	cmp    eax, 0
	je     __loop_break_0
	# for body
	# sum = (int)(sum) + (int)(x)
	# (int)(sum) + (int)(x)
	# (int)(sum)
	# sum
	lea    eax, dword ptr [ebp - 8]
	mov    eax, dword ptr [eax + 0]
	push   eax
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	mov    ebx, dword ptr [eax + 0]
	pop    eax
	add    eax, ebx
	push   eax
	# sum
	lea    eax, dword ptr [ebp - 8]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
__loop_continure0:
	jmp    __loop_second_plus_0
	# for end
__loop_break_0:
	# return sum
	# sum
	lea    eax, dword ptr [ebp - 8]
	mov    eax, dword ptr [eax + 0]
	jmp    __test_for_return
__test_for_return:
	add    esp, 8
	pop    ebp
	ret
	# Frame Size: 0
	# EBP        UID   SYMBOL     TYPE                
	# 8          0     x          int                 
	.globl _sum
_sum:
	push   ebp
	mov    ebp, esp
	sub    esp, 0
	# if
	# (int)(x) <= 0
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	push   eax
	# 0
	mov    eax, 0
	mov    ebx, eax
	pop    eax
	cmp    eax, ebx
	setle  al
	and    al, 1
	movzx  eax, al
	cmp    eax, 0
	je     __else_block_2
	# then
	# return 0
	# 0
	mov    eax, 0
	jmp    __sum_return
	jmp    __endif_2
	# else
__else_block_2:
	# return (int)(x) + (((int) -> int) *)(sum)((int)(x) - 1)
	# (int)(x) + (((int) -> int) *)(sum)((int)(x) - 1)
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	push   eax
	# (((int) -> int) *)(sum)((int)(x) - 1)
	# (int)(x) - 1
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	push   eax
	# 1
	mov    eax, 1
	mov    ebx, eax
	pop    eax
	sub    eax, ebx
	push   eax
	# (((int) -> int) *)(sum)
	# sum
	lea    eax, dword ptr [_sum + 0]
	call   eax
	add    esp, 4
	mov    ebx, eax
	pop    eax
	add    eax, ebx
	jmp    __sum_return
__endif_2:
__sum_return:
	add    esp, 0
	pop    ebp
	ret
	# Frame Size: 8
	# EBP        UID   SYMBOL     TYPE                
	# 12         1     argv       ((char) *) *        
	# 8          0     argc       int                 
	# -4         2     i          int                 
	# -8         3     j          int                 
	.globl _func1
_func1:
	push   ebp
	mov    ebp, esp
	sub    esp, 8
	# i = (int)(argc) - 4
	# (int)(argc) - 4
	# (int)(argc)
	# argc
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	push   eax
	# 4
	mov    eax, 4
	mov    ebx, eax
	pop    eax
	sub    eax, ebx
	push   eax
	# i
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# j = i++
	# i++
	# i
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, dword ptr [eax + 0]
	inc    dword ptr [eax + 0]
	mov    eax, ebx
	push   eax
	# j
	lea    eax, dword ptr [ebp - 8]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# j = (int)(i) <= 5
	# (int)(i) <= 5
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp - 4]
	mov    eax, dword ptr [eax + 0]
	push   eax
	# 5
	mov    eax, 5
	mov    ebx, eax
	pop    eax
	cmp    eax, ebx
	setle  al
	and    al, 1
	movzx  eax, al
	push   eax
	# j
	lea    eax, dword ptr [ebp - 8]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# return j
	# j
	lea    eax, dword ptr [ebp - 8]
	mov    eax, dword ptr [eax + 0]
	jmp    __func1_return
__func1_return:
	add    esp, 8
	pop    ebp
	ret
