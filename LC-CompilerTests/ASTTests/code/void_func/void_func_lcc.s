	.data
	.bss
	.text
	.intel_syntax noprefix
	# Frame Size: 8
	# EBP        UID   SYMBOL     TYPE                
	# 8          0     a          int                 
	# -4         1     x          unsigned int        
	# -8         2     y          unsigned int        
	.globl _foo
_foo:
	push   ebp
	mov    ebp, esp
	sub    esp, 8
	# a += 1
	# 1
	mov    eax, 1
	push   eax
	# a
	lea    eax, dword ptr [ebp + 8]
	pop    ebx
	push   eax
	mov    eax, dword ptr [eax + 0]
	add    eax, ebx
	pop    ebx
	mov    dword ptr [ebx + 0], eax
	# a += 1
	# 1
	mov    eax, 1
	push   eax
	# a
	lea    eax, dword ptr [ebp + 8]
	pop    ebx
	push   eax
	mov    eax, dword ptr [eax + 0]
	add    eax, ebx
	pop    ebx
	mov    dword ptr [ebx + 0], eax
	# x = (unsigned int)(1)
	# (unsigned int)(1)
	# 1
	mov    eax, 1
	push   eax
	# x
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# y = (unsigned int)(2)
	# (unsigned int)(2)
	# 2
	mov    eax, 2
	push   eax
	# y
	lea    eax, dword ptr [ebp - 8]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# x = % ((unsigned int)(x)) ((unsigned int)(y))
	# % ((unsigned int)(x)) ((unsigned int)(y))
	# (unsigned int)(x)
	# x
	lea    eax, dword ptr [ebp - 4]
	push   dword ptr [eax + 0]
	# (unsigned int)(y)
	# y
	lea    eax, dword ptr [ebp - 8]
	mov    ebx, dword ptr [eax + 0]
	pop    eax
	xor    edx, edx
	div    ebx
	mov    eax, edx
	push   eax
	# x
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# return a
	# a
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	jmp    __foo_return
__foo_return:
	add    esp, 8
	pop    ebp
	ret
	# Frame Size: 0
	# EBP        UID   SYMBOL     TYPE                
	# 8          0     x          int                 
	.globl _test_if
_test_if:
	push   ebp
	mov    ebp, esp
	sub    esp, 0
	# if
	# <= ((int)(x)) (0)
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
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
	# <= ((int)(x)) (2)
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
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
	# return + ((int)(x)) (1)
	# + ((int)(x)) (1)
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
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
	# return - ((int)(x)) (4)
	# - ((int)(x)) (4)
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
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
	# < ((int)(i)) ((int)(x))
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp - 4]
	push   dword ptr [eax + 0]
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
	# sum = + ((int)(sum)) ((int)(x))
	# + ((int)(sum)) ((int)(x))
	# (int)(sum)
	# sum
	lea    eax, dword ptr [ebp - 8]
	push   dword ptr [eax + 0]
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
	# <= ((int)(x)) (0)
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
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
	# return + ((int)(x)) ((((int) -> int) *)(sum)(- ((int)(x)) (1)))
	# + ((int)(x)) ((((int) -> int) *)(sum)(- ((int)(x)) (1)))
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# (((int) -> int) *)(sum)(- ((int)(x)) (1))
	# - ((int)(x)) (1)
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
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
	# i = - ((int)(argc)) (4)
	# - ((int)(argc)) (4)
	# (int)(argc)
	# argc
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
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
	# j = <= ((int)(i)) (5)
	# <= ((int)(i)) (5)
	# (int)(i)
	# i
	lea    eax, dword ptr [ebp - 4]
	push   dword ptr [eax + 0]
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
	# Frame Size: 4
	# EBP        UID   SYMBOL     TYPE                
	# 8          0     x          int                 
	# -4         1     j          int                 
	.globl _test_mul
_test_mul:
	push   ebp
	mov    ebp, esp
	sub    esp, 4
	# j = * ((int)(x)) (5)
	# * ((int)(x)) (5)
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# 5
	mov    eax, 5
	mov    ebx, eax
	pop    eax
	imul   eax, ebx
	push   eax
	# j
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# return j
	# j
	lea    eax, dword ptr [ebp - 4]
	mov    eax, dword ptr [eax + 0]
	jmp    __test_mul_return
__test_mul_return:
	add    esp, 4
	pop    ebp
	ret
	# Frame Size: 4
	# EBP        UID   SYMBOL     TYPE                
	# 8          0     x          int                 
	# -4         1     j          int                 
	.globl _test_div
_test_div:
	push   ebp
	mov    ebp, esp
	sub    esp, 4
	# j = / ((int)(x)) (2)
	# / ((int)(x)) (2)
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# 2
	mov    eax, 2
	mov    ebx, eax
	pop    eax
	cdq
	idiv   ebx
	push   eax
	# j
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# return j
	# j
	lea    eax, dword ptr [ebp - 4]
	mov    eax, dword ptr [eax + 0]
	jmp    __test_div_return
__test_div_return:
	add    esp, 4
	pop    ebp
	ret
	# Frame Size: 0
	# EBP        UID   SYMBOL     TYPE                
	# 8          0     x          int                 
	.globl _test_log_and
_test_log_and:
	push   ebp
	mov    ebp, esp
	sub    esp, 0
	# if
	# && (> ((int)(x)) (0)) (< ((int)(x)) (5))
	# > ((int)(x)) (0)
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# 0
	mov    eax, 0
	mov    ebx, eax
	pop    eax
	cmp    eax, ebx
	setg   al
	and    al, 1
	movzx  eax, al
	cmp    eax, 0
	je     __logical_shortcut_0
	# < ((int)(x)) (5)
	# (int)(x)
	# x
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# 5
	mov    eax, 5
	mov    ebx, eax
	pop    eax
	cmp    eax, ebx
	setl   al
	and    al, 1
	movzx  eax, al
	cmp    eax, 0
	je     __logical_shortcut_0
	mov    eax, 1
	jmp    __logical_end_0
__logical_shortcut_0:
	mov    eax, 0
__logical_end_0:
	cmp    eax, 0
	je     __else_block_3
	# then
	# return 1
	# 1
	mov    eax, 1
	jmp    __test_log_and_return
	jmp    __endif_3
	# else
__else_block_3:
	# return 0
	# 0
	mov    eax, 0
	jmp    __test_log_and_return
__endif_3:
__test_log_and_return:
	add    esp, 0
	pop    ebp
	ret
