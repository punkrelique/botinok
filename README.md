# Botinok 

Telegram bot that searches and downloads books documents from Libgen straight into chat

## Configuration and build

### Docker

Build docker image:

```bash
docker build --tag punkrelique/botinok src/ -f src/Botinok/Dockerfile
```

or just pull ready image from docker registry: 
```bash
docker pull punkrelique/botinok
```

### Settings
```json
{
  "Bot": {
    "Token": ""
  }
}
```

## Available commands

- /start
- /search book_name
