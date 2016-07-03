dim b as string
For i = 1 to 9
	b = b & i
next
dim h as integer, g as integer
g = 10
for i = 1 to g step 0
	g = g - 1
next
dim c as string
for i = 9 to 1 step -1
	c = c & i
next
for i = 10 to 1000000
	msgbox "will execute\n" & b & " " & g & "\n" & c
	exit for
	msgbox "\n will not execute"
next
