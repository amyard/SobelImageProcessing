## Run Project
type in terminal    
```
git clone https://amyard@bitbucket.org/amyard/sobelimageprocessing.git
cd sobelimageprocessing/SobelAlgImage
dotnet watch run
```

it will be using https://localhost:5001  

Or
open project in Visual Studio and run the solution.

## Task Description
Требуется написать программу с использованием языка программирования C#, которая осуществляет загрузку изображения, а затем его обработку при помощи оператора Собеля, предназначенного для выделения границ объектов.    

При выполнении задания необходимо учесть следующее:
- Для ускорения процесса получения результата разбить изображение на несколько частей и проводить параллельную их обработку.
- Результат выводить только после завершения обработки изображения несколькими потоками.
- Количество частей изображения, которые будут обрабатываться параллельно, может быть введено пользователем системы (к примеру, ввод с консоли).

## Project description
1. Section "Load file": 
- if "Amount of parallel processes" left empty it will parse uploaded image with 4 processes;
- you can import images with format "png", "jpg", "jpeg", "bmp", "webp".
2. Section "Display Result":
- it will display 5 images: original image, 4 images after Sobel images processing with different parameters.
- delete action button will delete model with corresponded images.