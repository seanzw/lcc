	.data
	.bss
	.text
	.intel_syntax noprefix
	# Frame Size: 4
	# EBP        UID   SYMBOL     TYPE                
	# 8          0     max        unsigned int        
	# -4         1     queue      (struct Queue) *    
	.globl _queue_init
_queue_init:
	push   ebp
	mov    ebp, esp
	sub    esp, 4
	# queue = ((struct Queue) *)((((unsigned int) -> (void) *) *)(malloc)(16))
	# ((struct Queue) *)((((unsigned int) -> (void) *) *)(malloc)(16))
	# (((unsigned int) -> (void) *) *)(malloc)(16)
	# 16
	mov    eax, 16
	push   eax
	# (((unsigned int) -> (void) *) *)(malloc)
	# malloc
	lea    eax, dword ptr [_malloc + 0]
	call   eax
	add    esp, 4
	push   eax
	# queue
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# if
	# != (((struct Queue) *)(queue)) (((struct Queue) *)0)
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp - 4]
	push   dword ptr [eax + 0]
	# ((struct Queue) *)0
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
	# (((struct Queue) *)(queue))->arr = ((int) *)((((unsigned int) -> (void) *) *)(malloc)(* (+ ((unsigned int)(max)) ((unsigned int)(1))) (4)))
	# ((int) *)((((unsigned int) -> (void) *) *)(malloc)(* (+ ((unsigned int)(max)) ((unsigned int)(1))) (4)))
	# (((unsigned int) -> (void) *) *)(malloc)(* (+ ((unsigned int)(max)) ((unsigned int)(1))) (4))
	# * (+ ((unsigned int)(max)) ((unsigned int)(1))) (4)
	# + ((unsigned int)(max)) ((unsigned int)(1))
	# (unsigned int)(max)
	# max
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# (unsigned int)(1)
	# 1
	mov    eax, 1
	mov    ebx, eax
	pop    eax
	add    eax, ebx
	push   eax
	# 4
	mov    eax, 4
	mov    ebx, eax
	pop    eax
	mul    ebx
	push   eax
	# (((unsigned int) -> (void) *) *)(malloc)
	# malloc
	lea    eax, dword ptr [_malloc + 0]
	call   eax
	add    esp, 4
	push   eax
	# (((struct Queue) *)(queue))->arr
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp - 4]
	mov    eax, dword ptr [eax + 0]
	add    eax, 0
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# if
	# != (((int) *)((((struct Queue) *)(queue))->arr)) (((int) *)0)
	# ((int) *)((((struct Queue) *)(queue))->arr)
	# (((struct Queue) *)(queue))->arr
	# ((struct Queue) *)(queue)
	# queue
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
	je     __else_block_0
	# then
	# (((struct Queue) *)(queue))->size = + ((unsigned int)(max)) ((unsigned int)(1))
	# + ((unsigned int)(max)) ((unsigned int)(1))
	# (unsigned int)(max)
	# max
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# (unsigned int)(1)
	# 1
	mov    eax, 1
	mov    ebx, eax
	pop    eax
	add    eax, ebx
	push   eax
	# (((struct Queue) *)(queue))->size
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp - 4]
	mov    eax, dword ptr [eax + 0]
	add    eax, 12
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# (((struct Queue) *)(queue))->head = (((struct Queue) *)(queue))->tail = (unsigned int)(0)
	# (((struct Queue) *)(queue))->tail = (unsigned int)(0)
	# (unsigned int)(0)
	# 0
	mov    eax, 0
	push   eax
	# (((struct Queue) *)(queue))->tail
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp - 4]
	mov    eax, dword ptr [eax + 0]
	add    eax, 8
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	push   eax
	# (((struct Queue) *)(queue))->head
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp - 4]
	mov    eax, dword ptr [eax + 0]
	add    eax, 4
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	jmp    __endif_0
	# else
__else_block_0:
	# ((((void) *) -> void) *)(free)(((void) *)(((struct Queue) *)(queue)))
	# ((void) *)(((struct Queue) *)(queue))
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp - 4]
	push   dword ptr [eax + 0]
	# ((((void) *) -> void) *)(free)
	# free
	lea    eax, dword ptr [_free + 0]
	call   eax
	add    esp, 4
__endif_0:
	jmp    __endif_1
__endif_1:
	# return queue
	# queue
	lea    eax, dword ptr [ebp - 4]
	mov    eax, dword ptr [eax + 0]
	jmp    __queue_init_return
__queue_init_return:
	add    esp, 4
	pop    ebp
	ret
	# Frame Size: 0
	# EBP        UID   SYMBOL     TYPE                
	# 8          0     queue      (struct Queue) *    
	.globl _queue_free
_queue_free:
	push   ebp
	mov    ebp, esp
	sub    esp, 0
	# if
	# != (((struct Queue) *)(queue)) (((struct Queue) *)0)
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# ((struct Queue) *)0
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
	# ((((void) *) -> void) *)(free)(((void) *)(((struct Queue) *)(queue)))
	# ((void) *)(((struct Queue) *)(queue))
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# ((((void) *) -> void) *)(free)
	# free
	lea    eax, dword ptr [_free + 0]
	call   eax
	add    esp, 4
	jmp    __endif_2
__endif_2:
__queue_free_return:
	add    esp, 0
	pop    ebp
	ret
	# Frame Size: 0
	# EBP        UID   SYMBOL     TYPE                
	# 8          0     queue      (struct Queue) *    
	.globl _queue_is_empty
_queue_is_empty:
	push   ebp
	mov    ebp, esp
	sub    esp, 0
	# return == ((unsigned int)((((struct Queue) *)(queue))->head)) ((unsigned int)((((struct Queue) *)(queue))->tail))
	# == ((unsigned int)((((struct Queue) *)(queue))->head)) ((unsigned int)((((struct Queue) *)(queue))->tail))
	# (unsigned int)((((struct Queue) *)(queue))->head)
	# (((struct Queue) *)(queue))->head
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 4
	push   dword ptr [eax + 0]
	# (unsigned int)((((struct Queue) *)(queue))->tail)
	# (((struct Queue) *)(queue))->tail
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 8
	mov    ebx, dword ptr [eax + 0]
	pop    eax
	cmp    eax, ebx
	sete   al
	and    al, 1
	movzx  eax, al
	jmp    __queue_is_empty_return
__queue_is_empty_return:
	add    esp, 0
	pop    ebp
	ret
	# Frame Size: 4
	# EBP        UID   SYMBOL     TYPE                
	# 12         1     element    int                 
	# 8          0     queue      (struct Queue) *    
	# -4         2     next       unsigned int        
	.globl _queue_enqueue
_queue_enqueue:
	push   ebp
	mov    ebp, esp
	sub    esp, 4
	# next = % (+ ((unsigned int)((((struct Queue) *)(queue))->tail)) ((unsigned int)(1))) ((unsigned int)((((struct Queue) *)(queue))->size))
	# % (+ ((unsigned int)((((struct Queue) *)(queue))->tail)) ((unsigned int)(1))) ((unsigned int)((((struct Queue) *)(queue))->size))
	# + ((unsigned int)((((struct Queue) *)(queue))->tail)) ((unsigned int)(1))
	# (unsigned int)((((struct Queue) *)(queue))->tail)
	# (((struct Queue) *)(queue))->tail
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 8
	push   dword ptr [eax + 0]
	# (unsigned int)(1)
	# 1
	mov    eax, 1
	mov    ebx, eax
	pop    eax
	add    eax, ebx
	push   eax
	# (unsigned int)((((struct Queue) *)(queue))->size)
	# (((struct Queue) *)(queue))->size
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 12
	mov    ebx, dword ptr [eax + 0]
	pop    eax
	xor    edx, edx
	div    ebx
	mov    eax, edx
	push   eax
	# next
	lea    eax, dword ptr [ebp - 4]
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# if
	# == ((unsigned int)(next)) ((unsigned int)((((struct Queue) *)(queue))->head))
	# (unsigned int)(next)
	# next
	lea    eax, dword ptr [ebp - 4]
	push   dword ptr [eax + 0]
	# (unsigned int)((((struct Queue) *)(queue))->head)
	# (((struct Queue) *)(queue))->head
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 4
	mov    ebx, dword ptr [eax + 0]
	pop    eax
	cmp    eax, ebx
	sete   al
	and    al, 1
	movzx  eax, al
	cmp    eax, 0
	je     __endif_3
	# then
	# return -1
	# -1
	mov    eax, -1
	jmp    __queue_enqueue_return
	jmp    __endif_3
__endif_3:
	# (((int) *)((((struct Queue) *)(queue))->arr))[(((struct Queue) *)(queue))->tail] = (int)(element)
	# (int)(element)
	# element
	lea    eax, dword ptr [ebp + 12]
	push   eax
	# (((int) *)((((struct Queue) *)(queue))->arr))[(((struct Queue) *)(queue))->tail]
	# ((int) *)((((struct Queue) *)(queue))->arr)
	# (((struct Queue) *)(queue))->arr
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 0
	push   dword ptr [eax + 0]
	# (((struct Queue) *)(queue))->tail
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 8
	mov    eax, dword ptr [eax + 0]
	mov    ebx, 4
	mul    ebx
	mov    ebx, eax
	pop    eax
	add    eax, ebx
	mov    ebx, eax
	pop    eax
	mov    eax, dword ptr [eax + 0]
	mov    dword ptr [ebx + 0], eax
	# (((struct Queue) *)(queue))->tail = (unsigned int)(next)
	# (unsigned int)(next)
	# next
	lea    eax, dword ptr [ebp - 4]
	push   eax
	# (((struct Queue) *)(queue))->tail
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 8
	mov    ebx, eax
	pop    eax
	mov    eax, dword ptr [eax + 0]
	mov    dword ptr [ebx + 0], eax
	# return 0
	# 0
	mov    eax, 0
	jmp    __queue_enqueue_return
__queue_enqueue_return:
	add    esp, 4
	pop    ebp
	ret
	# Frame Size: 0
	# EBP        UID   SYMBOL     TYPE                
	# 12         1     dst        (int) *             
	# 8          0     queue      (struct Queue) *    
	.globl _queue_dequeue
_queue_dequeue:
	push   ebp
	mov    ebp, esp
	sub    esp, 0
	# if
	# ((((struct Queue) *) -> int) *)(queue_is_empty)(((struct Queue) *)(queue))
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp + 8]
	push   dword ptr [eax + 0]
	# ((((struct Queue) *) -> int) *)(queue_is_empty)
	# queue_is_empty
	lea    eax, dword ptr [_queue_is_empty + 0]
	call   eax
	add    esp, 4
	cmp    eax, 0
	je     __endif_4
	# then
	# return -1
	# -1
	mov    eax, -1
	jmp    __queue_dequeue_return
	jmp    __endif_4
__endif_4:
	# *(((int) *)(dst)) = (int)((((int) *)((((struct Queue) *)(queue))->arr))[(((struct Queue) *)(queue))->head])
	# (int)((((int) *)((((struct Queue) *)(queue))->arr))[(((struct Queue) *)(queue))->head])
	# (((int) *)((((struct Queue) *)(queue))->arr))[(((struct Queue) *)(queue))->head]
	# ((int) *)((((struct Queue) *)(queue))->arr)
	# (((struct Queue) *)(queue))->arr
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 0
	push   dword ptr [eax + 0]
	# (((struct Queue) *)(queue))->head
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 4
	mov    eax, dword ptr [eax + 0]
	mov    ebx, 4
	mul    ebx
	mov    ebx, eax
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
	# (((struct Queue) *)(queue))->head = % (+ ((unsigned int)((((struct Queue) *)(queue))->head)) ((unsigned int)(1))) ((unsigned int)((((struct Queue) *)(queue))->size))
	# % (+ ((unsigned int)((((struct Queue) *)(queue))->head)) ((unsigned int)(1))) ((unsigned int)((((struct Queue) *)(queue))->size))
	# + ((unsigned int)((((struct Queue) *)(queue))->head)) ((unsigned int)(1))
	# (unsigned int)((((struct Queue) *)(queue))->head)
	# (((struct Queue) *)(queue))->head
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 4
	push   dword ptr [eax + 0]
	# (unsigned int)(1)
	# 1
	mov    eax, 1
	mov    ebx, eax
	pop    eax
	add    eax, ebx
	push   eax
	# (unsigned int)((((struct Queue) *)(queue))->size)
	# (((struct Queue) *)(queue))->size
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 12
	mov    ebx, dword ptr [eax + 0]
	pop    eax
	xor    edx, edx
	div    ebx
	mov    eax, edx
	push   eax
	# (((struct Queue) *)(queue))->head
	# ((struct Queue) *)(queue)
	# queue
	lea    eax, dword ptr [ebp + 8]
	mov    eax, dword ptr [eax + 0]
	add    eax, 4
	mov    ebx, eax
	pop    eax
	mov    dword ptr [ebx + 0], eax
	# return 0
	# 0
	mov    eax, 0
	jmp    __queue_dequeue_return
__queue_dequeue_return:
	add    esp, 0
	pop    ebp
	ret
