Задача состоит в написании программы, которая получает данные из xml-файла, содержащего линейный список записей, и 
сохраняет их в корректный xml файл, имеющий иерархическую структуру.

**Входной xml файл имеет следующий вид:**
```XML
<?xml version="1.0" encoding="utf-8" ?>
<root>
	<item id="..." parentId="..."> ... </item>
	<item id="..." parentId="..."> ... </item>
	<item id="..." parentId="..."> ... </item>
	<item id="..." parentId="..."> ... </item>
	<item id="..." parentId="..."> ... </item>
</root>
```
**Описание атрибутов:**
1. id - идентификатор записи
1. parentId - id записи, в которую эта запись вложена.
1. text - значение элемента <item> (т.е. то, что находится между <item> и </item>)

**Выходной файл должен иметь следующий вид:**
```XML
<?xml version="1.0" encoding="utf-8" ?>
<root>
	<item id="..." text="...">
		<item id="..." text="...">
			<item id="..." text="..." />
			<item id="..." text="..." />
		</item>
	</item>
</root>
```
**Требования к выходному файлу:**
1. Каждой входной записи должна соответствовать одна и только одна запись в выходном файле.
1. Необходимо по возможности максимально точно сохранить информацию о вложенности записей.

Чем меньше дополнительных ограничений будет наложено на входные данные, тем лучше. Язык - C#.
