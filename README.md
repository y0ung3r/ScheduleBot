# ScheduleBot
![Build Status](https://github.com/y0ung3r/ScheduleBot/actions/workflows/dotnet-desktop.yml/badge.svg?branch=master)

Бот, предназначенный для получения актуального расписания СФ БашГУ

## Сборка и запуск бота
На данный момент бот может работать только в Telegram.

#### Telegram
1. Создайте API-ключ для бота, используя BotFather;
2. Для подключения бота к Telegram Bot API, в папке **ScheduleBot.Telegram/Configurations** создайте файл **Bot.config** со следующим содержимым:
```xml
<!-- Вместо *API_TOKEN* вставьте свой API-ключ -->
<appSettings>
   <add key="token" value="API_TOKEN" />
</appSettings>
```

## Использованные библиотеки
- [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot "Telegram.Bot") 
- [Html Agility Pack](https://github.com/zzzprojects/html-agility-pack "Html Agility Pack")
- [NUnit](https://github.com/nunit/nunit "NUnit")
