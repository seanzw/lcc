	.data
	.text
	.intel_syntax noprefix
	# Frame Size: 44
	# EBP        UID   SYMBOL     TYPE                
	# 12         1     argv       ((char) *) *        
	# 8          0     argc       int                 
	# -4         2     a          int                 
	# -44        3     c          (int)[10]           
	# -44        4     d          (int)[10]           
	.globl _func1
_func1:
	push   ebp
	mov    ebp, esp
	sub    esp, 44
	# c
	lea    eax, [ebp - 44]
	# argc
	lea    eax, [ebp + 8]
	# a
	lea    eax, [ebp - 4]
__func1_return:
	add    esp, 44
	pop    ebp
	ret
	# Frame Size: 4
	# EBP        UID   SYMBOL     TYPE                
	# 12         1     argv       ((char) *) *        
	# 8          0     argc       int                 
	# -4         2     a          char                
	.globl _func2
_func2:
	push   ebp
	mov    ebp, esp
	sub    esp, 4
__func2_return:
	add    esp, 4
	pop    ebp
	ret
