Название: Подмножество языка VisualBasic, ссылка: https://ru.wikipedia.org/wiki/Visual_Basic

Сложность: For .. To .. Step (Exit For) .. Next

Операторы: = (присвоение), +, -, *, /, ^, \, Mod, MsgBox, Dim .. As, & (оператор конкатенации)

Типы: Single, Integer, Decimal

Пример того что нужно сделать:

```
Dim i As Integer
Dim a As Integer
a=2
For i=0 To 100 Step 2
a=a+5
Next
MsgBox "a=" & a
```

Вывод:

```
a=257
```
