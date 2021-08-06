# ScheduleBot
![Build Status](https://github.com/y0ung3r/ScheduleBot/actions/workflows/dotnet-desktop.yml/badge.svg?branch=master)

Бот, предназначенный для получения актуального расписания СФ БашГУ

## Структура
Проект **ScheduleBot** содержит логику построения цепочки обработчиков входящих сообщений на основе паттерна проектирования *Цепочка обязанностей*, а также классы, необходимые для создания функциональности бота.

Проект **ScheduleBot.Parser** содержит парсер сайта http://edu.strbsu.ru/.

Проект **ScheduleBot.Data** содержит инфраструктуру для работы с базой данных.

Проект **ScheduleBot.Domain** содержит логику бота, независимую от конкретной платформы, на которой бот может работать.

Проект **ScheduleBot.Telegram** содержит классы для работы бота в Telegram.

## Сборка и запуск бота
На данный момент бот может работать только в Telegram. В качестве СУБД используется MS SQL Server.

#### Telegram
1. Создайте API-ключ для бота, используя BotFather;
2. Для подключения бота к Telegram Bot API, в папке **ScheduleBot.Telegram/Configurations** создайте файл **Bot.config** со следующим содержимым:
```xml
<!-- Вместо *API_TOKEN* вставьте свой API-ключ -->
<appSettings>
   <add key="token" value="API_TOKEN" />
</appSettings>
```
3. Оставаясь в директории **ScheduleBot.Telegram/Configurations**, создайте файл **Database.config**, который должен содержать строку подключения к базе данных MS SQL Server:
```xml
<connectionStrings>
   <!-- Вместо *CONNECTION_STRING* вставьте свою строку подключения к базе данных -->
   <add name="ScheduleBot"
        connectionString="CONNECTION_STRING" 
        providerName="System.Data.SqlClient" />
</connectionStrings>
```
4. В свойствах **Bot.config** и **Database.config** параметру *Копировать в выходной каталог* присвойте значение *Всегда копировать*.

## Использованные библиотеки
- [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot "Telegram.Bot") 
- [Html Agility Pack](https://github.com/zzzprojects/html-agility-pack "Html Agility Pack")
- [NUnit](https://github.com/nunit/nunit "NUnit")
