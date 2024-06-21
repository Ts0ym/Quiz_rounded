# Викторина "Углеродный след"

## Обзор
Этот проект на Unity представляет собой приложение-викторину, разработанное для выставки ВДНХ-Россия 2024, 
Пользователь отвечает на вопросы и в результате видит количество потребляемого им CO2 в год. 
Вопросы загружаются из JSON файла.

## Особенности
- Адаптирован по круглий дисплей с поддержкой сенсорного управления
- Загрузка вопросов из JSON файла
- По мере прохождения викторины окружение планеты меняется

## Требования
- Unity 2021.3 или более поздняя версия
- Newtonsoft.Json для парсинга JSON (установить через Unity Package Manager)

## Начало работы

### Клонирование репозитория
Клонируйте этот репозиторий на свой компьютер с помощью команды:
```sh
git clone https://github.com/Ts0ym/Quiz_rounded.git
```

## Открытие в Unity
Откройте Unity Hub.
Нажмите на Открыть, найдите склонированную папку репозитория и выберите её.
Дождитесь загрузки проекта и разрешения зависимостей в Unity.

## Запуск билда на Windows без установки Unity
1. Перейдите в папку Builds
2. Запустите файл Quiz_rounded2.exe

## Использование
1. Запустите приложение на вашем устройстве.
3. Отвечайте на вопросы, отображаемые на экране.
4. Просмотрите свой счет и узнайте, свой углеродный след выбрасываемый в атмосферу.

## Изменение вопросов
- Вопросы расположены в файле Assets/questions.json

### Структура объектов вопросов
{
		"question": "*Текст вопроса*",
		"answerKeys": ["вариант ответа 1", "вариант ответа 2", "вариант ответа n"],
		"answerValues": [0.2997, 0.524475, 0.74925] #Значения CO2 для каждого из вариантов ответа
	}
