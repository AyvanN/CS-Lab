# CS-Lab
Папка Programs содержит классы, которые использовались в данной лабораторной работе.
<h3>Описание классов:</h3>
<p>1) Класс ArchiveBuilder позволяет выполнять операции архивирования и деархивирования.<br>
<p>2)В класс ArchiveTools были объединены настройки(пути к директории) для работы с архивацией файлов.<br>
<p>3)Класс EncryptTools позволяет выполнять операции шифрования и дешифрования при помощи Rijndael algorithm - симметричный алгоритм блочного шифрования.<br>
<p>4)В класс Tools были помещены переменные, в которых содержится путь к нашим директориям, который мы получим из json и xml файлов.<br>
<p>5)Класс FileLogger позволяет логировать все операции которые происходят при выполнении нашего сервиса. Все исключительные ситуации обрабатываются и логируются в файл Exception.txt. При этом, если данного файла не существовало ранее, то он создается, а если файл был уже создан, то новые данные просто дописываются в его конец. При помощи данного класса мы можем запускать и останавливать наш сервис,а также были добавлены функции отслеживание и переименования файла . Все действия будут записываться в стандартный файл.<br>
<p>6)Класс FileInstaller используется для установки службы.<br>
<p>7)Классы XMLParser и JsonParser содержат описание парсеров xml и json. Данные парсеры являются универсальными, т.е изменение настроек файлов(добавление нового объекта или поля в json или нового тега\секции в XML) не повлечёт за собой изменения парсера.
Каждый парсер наследуется от интерфейса IParser, который в свою очередь описывается одним полем (T Parse<T>()).<br>
<p>8)В классе FileInstallation описаны события OnStart(), где происходит процесс валидации xml файла с помощью xsd файла, и OnStop().<br>
<p>9)Класс CheckingFiles проверяет тип конфигурационного файла (XML/JSON) и определяет какой конфигурационный файл нужно использовать. Данный класс получает путь к файлу(с использованием AppDomain), если же подходящий файл не найдет, то вылетает исключение.<br>


<h3>Описание файлов:</h3>

<p>1)JsonFile.json - файл, который является экземпляром класса Tools, написанный в формате json.<br>
<p>2)XMLFile.xml - файл, который является экземпляром класса Tools, написанный в формате xml.<br>
<p>3)XMLFile.xds - файл, с помощью которого происходит валидация файла XMLFile.xml.<br>
