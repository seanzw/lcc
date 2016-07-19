	.data
	.bss
	.text
	.intel_syntax noprefix
	# Frame Size: 4
	# EBP        UID   SYMBOL     TYPE                
	# 8          0     max        unsigned int        
	# -4         1     stack      (struct struct@0) * 
	.globl _stack_array_initialize
_stack_array_initialize:
	push   ebp
	mov    ebp, esp
	sub    esp, 4
	# stack = ((struct struct@0) *)((((unsigned int) -> (void) *) *)(malloc)(12))
	# ((struct struct@0) *)((((unsigned int) -> (void) *) *)(malloc)(12))
	# (((unsigned int) -> (void) *) *)(malloc)(12)
	# 12
	mov    eax, 12
	push   eax
	# (((unsigned int) -> (void) *) *)(malloc)
	# malloc
	lea    eax, dword ptr [_malloc + 0]
	call   eax
	add    esp, 4
	push   eax
	# stack
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# if
	# != (((struct struct@0) *)(stack)) (((struct struct@0) *)0)
	# ((struct struct@0) *)(stack)
	# stack
	lea    eax, dword ptr [ebp - 4]
	push   dword ptr [eax + 0]
	# ((struct struct@0) *)0
	xor    eax, eax
	mov    ebx, eax
	pop    eax
	cmp    eax, ebx
	setne  al
	and    al, 1
	movzx  eax, al
	cmp    eax, 0
	je     __endif_1
	# then
	# (((struct struct@0) *)(stack))->arr = ((int) *)((((unsigned int) -> (void) *) *)(malloc)(* (4) ((unsigned int)(max))))
	# ((int) *)((((unsigned int) -> (void) *) *)(malloc)(* (4) ((unsigned int)(max))))
	# (((unsigned int) -> (void) *) *)(malloc)(* (4) ((unsigned int)(max)))
	# * (4) ((unsigned int)(max))
	# 4
	mov    eax, 4
	push   eax
	# (unsigned int)(max)
	# max
	lea    eax, dword ptr [ebp + 8]
	mov    ebx, dword ptr [eax + 0]
	pop    eax
	mul    ebx
	push   eax
	# (((unsigned int) -> (void) *) *)(malloc)
	# malloc
	lea    eax, dword ptr [_malloc + 0]
	call   eax
	add    esp, 4
	push   eax
	# (((struct struct@0) *)(stack))->arr
	# ((struct struct@0) *)(stack)
	# stack
	lea    eax, dword ptr [ebp - 4]
	mov    eax, dword ptr [eax + 0]
	add    eax, 0
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# if
	# != (((int) *)((((struct struct@0) *)(stack))->arr)) (((int) *)0)
	# ((int) *)((((struct struct@0) *)(stack))->arr)
	# (((struct struct@0) *)(stack))->arr
	# ((struct struct@0) *)(stack)
	# stack
	lea    eax, dword ptr [ebp - 4]
	mov    eax, dword ptr [eax + 0]
	add    eax, 0
	push   dword ptr [eax + 0]
	# ((int) *)0
	xor    eax, eax
	mov    ebx, eax
	pop    eax
	cmp    eax, ebx
	setne  al
	and    al, 1
	movzx  eax, al
	cmp    eax, 0
	je     __endif_0
	# then
	# (((struct struct@0) *)(stack))->top = 0
	# 0
	mov    eax, 0
	push   eax
	# (((struct struct@0) *)(stack))->top
	# ((struct struct@0) *)(stack)
	# stack
	lea    eax, dword ptr [ebp - 4]
	mov    eax, dword ptr [eax + 0]
	add    eax, 4
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# (((struct struct@0) *)(stack))->max = (int)((unsigned int)(max))
	# (int)((unsigned int)(max))
	# (unsigned int)(max)
	# max
	lea    eax, dword ptr [ebp + 8]
	push   eax
	# (((struct struct@0) *)(stack))->max
	# ((struct struct@0) *)(stack)
	# stack
	lea    eax, dword ptr [ebp - 4]
	mov    eax, dword ptr [eax + 0]
	add    eax, 8
	mov    ebx, eax
	pop    eax
	mov    eax, dword ptr [eax + 0]
	mov    dword ptr [ebx + 0], eax
	# return stack
	# stack
	lea    eax, dword ptr [ebp - 4]
	mov    eax, dword ptr [eax + 0]
	jmp    __stack_array_initialize_return
	jmp    __endif_0
__endif_0:
	jmp    __endif_1
__endif_1:
	# return ((struct struct@0) *)0
	# ((struct struct@0) *)0
	xor    eax, eax
	jmp    __stack_array_initialize_return
__stack_array_initialize_return:
	add    esp, 4
	pop    ebp
	ret
	# Frame Size: 0
	# EBP        UID   SYMBOL     TYPE                
	# 8          0     stack      (struct struct@0) * 
	.globl _stack_array_destroy
_stack_array_destroy:
	push   ebp
	mov    ebp, esp
	sub    esp, 0
	# if
	# != (((struct struct@0) *)(stack)) (((struct struct@0) *)0)
	# ((struct struct@0) *)(stack)
	# stack
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# ((struct struct@0) *)0
	xor    eax, eax
	mov    ebx, eax
	pop    eax
	cmp    eax, ebx
	setne  al
	and    al, 1
	movzx  eax, al
	cmp    eax, 0
	je     __endif_2
	# then
	# ((((void) *) -> void) *)(free)(((void) *)(((int) *)((((struct struct@0) *)(stack))->arr)))
	# ((void) *)(((int) *)((((struct struct@0) *)(stack))->arr))
	# ((int) *)((((struct struct@0) *)(stack))->arr)
	# (((struct struct@0) *)(stack))->arr
	# ((struct struct@0) *)(stack)
	# stack
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 0
	push   dword ptr [eax + 0]
	# ((((void) *) -> void) *)(free)
	# free
	lea    eax, dword ptr [_free + 0]
	call   eax
	add    esp, 4
	# ((((void) *) -> void) *)(free)(((void) *)(((struct struct@0) *)(stack)))
	# ((void) *)(((struct struct@0) *)(stack))
	# ((struct struct@0) *)(stack)
	# stack
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# ((((void) *) -> void) *)(free)
	# free
	lea    eax, dword ptr [_free + 0]
	call   eax
	add    esp, 4
	jmp    __endif_2
__endif_2:
__stack_array_destroy_return:
	add    esp, 0
	pop    ebp
	ret
	# Frame Size: 0
	# EBP        UID   SYMBOL     TYPE                
	# 8          0     stack      (struct struct@0) * 
	.globl _stack_array_empty
_stack_array_empty:
	push   ebp
	mov    ebp, esp
	sub    esp, 0
	# return == ((int)((((struct struct@0) *)(stack))->top)) (0)
	# == ((int)((((struct struct@0) *)(stack))->top)) (0)
	# (int)((((struct struct@0) *)(stack))->top)
	# (((struct struct@0) *)(stack))->top
	# ((struct struct@0) *)(stack)
	# stack
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 4
	push   dword ptr [eax + 0]
	# 0
	mov    eax, 0
	mov    ebx, eax
	pop    eax
	cmp    eax, ebx
	sete   al
	and    al, 1
	movzx  eax, al
	jmp    __stack_array_empty_return
__stack_array_empty_return:
	add    esp, 0
	pop    ebp
	ret
	# Frame Size: 0
	# EBP        UID   SYMBOL     TYPE                
	# 12         1     element    int                 
	# 8          0     stack      (struct struct@0) * 
	.globl _stack_array_push
_stack_array_push:
	push   ebp
	mov    ebp, esp
	sub    esp, 0
	# if
	# < ((int)((((struct struct@0) *)(stack))->top)) ((int)((((struct struct@0) *)(stack))->max))
	# (int)((((struct struct@0) *)(stack))->top)
	# (((struct struct@0) *)(stack))->top
	# ((struct struct@0) *)(stack)
	# stack
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 4
	push   dword ptr [eax + 0]
	# (int)((((struct struct@0) *)(stack))->max)
	# (((struct struct@0) *)(stack))->max
	# ((struct struct@0) *)(stack)
	# stack
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 8
	mov    ebx, dword ptr [eax + 0]
	pop    eax
	cmp    eax, ebx
	setl   al
	and    al, 1
	movzx  eax, al
	cmp    eax, 0
	je     __endif_3
	# then
	# (((int) *)((((struct struct@0) *)(stack))->arr))[(((struct struct@0) *)(stack))->top++] = (int)(element)
	# (int)(element)
	# element
	lea    eax, dword ptr [ebp + 12]
	push   eax
	# (((int) *)((((struct struct@0) *)(stack))->arr))[(((struct struct@0) *)(stack))->top++]
	# ((int) *)((((struct struct@0) *)(stack))->arr)
	# (((struct struct@0) *)(stack))->arr
	# ((struct struct@0) *)(stack)
	# stack
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 0
	push   dword ptr [eax + 0]
	# (((struct struct@0) *)(stack))->top++
	# (((struct struct@0) *)(stack))->top
	# ((struct struct@0) *)(stack)
	# stack
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 4
	mov    ebx, dword ptr [eax + 0]
	inc    dword ptr [eax + 0]
	mov    eax, ebx
	imul   ebx, eax, 4
	pop    eax
	add    eax, ebx
	mov    ebx, eax
	pop    eax
	mov    eax, dword ptr [eax + 0]
	mov    dword ptr [ebx + 0], eax
	# return 0
	# 0
	mov    eax, 0
	jmp    __stack_array_push_return
	jmp    __endif_3
__endif_3:
	# return -1
	# -1
	mov    eax, -1
	jmp    __stack_array_push_return
__stack_array_push_return:
	add    esp, 0
	pop    ebp
	ret
	# Frame Size: 0
	# EBP        UID   SYMBOL     TYPE                
	# 12         1     dst        (int) *             
	# 8          0     stack      (struct struct@0) * 
	.globl _stack_array_pop
_stack_array_pop:
	push   ebp
	mov    ebp, esp
	sub    esp, 0
	# if
	# ((((struct struct@0) *) -> int) *)(stack_array_empty)(((struct struct@0) *)(stack))
	# ((struct struct@0) *)(stack)
	# stack
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# ((((struct struct@0) *) -> int) *)(stack_array_empty)
	# stack_array_empty
	lea    eax, dword ptr [_stack_array_empty + 0]
	call   eax
	add    esp, 4
	cmp    eax, 0
	je     __endif_4
	# then
	# return -1
	# -1
	mov    eax, -1
	jmp    __stack_array_pop_return
	jmp    __endif_4
__endif_4:
	# *(((int) *)(dst)) = (int)((((int) *)((((struct struct@0) *)(stack))->arr))[(((struct struct@0) *)(stack))->top -= 1])
	# (int)((((int) *)((((struct struct@0) *)(stack))->arr))[(((struct struct@0) *)(stack))->top -= 1])
	# (((int) *)((((struct struct@0) *)(stack))->arr))[(((struct struct@0) *)(stack))->top -= 1]
	# ((int) *)((((struct struct@0) *)(stack))->arr)
	# (((struct struct@0) *)(stack))->arr
	# ((struct struct@0) *)(stack)
	# stack
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 0
	push   dword ptr [eax + 0]
	# (((struct struct@0) *)(stack))->top -= 1
	# 1
	mov    eax, 1
	push   eax
	# (((struct struct@0) *)(stack))->top
	# ((struct struct@0) *)(stack)
	# stack
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 4
	pop    ebx
	push   eax
	mov    eax, dword ptr [eax + 0]
	sub    eax, ebx
	pop    ebx
	mov    dword ptr [ebx + 0], eax
	imul   ebx, eax, 4
	pop    eax
	add    eax, ebx
	push   eax
	# *(((int) *)(dst))
	# ((int) *)(dst)
	# dst
	lea    eax, dword ptr [ebp + 12]
	mov    eax, dword ptr [eax + 0]
	mov    ebx, eax
	pop    eax
	mov    eax, dword ptr [eax + 0]
	mov    dword ptr [ebx + 0], eax
	# return 0
	# 0
	mov    eax, 0
	jmp    __stack_array_pop_return
__stack_array_pop_return:
	add    esp, 0
	pop    ebp
	ret
