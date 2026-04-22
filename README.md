# 🎬 AdLume

> Transforme suas telas em receita.

**AdLume** é uma plataforma de **Digital Signage** focada em automação de exibição de conteúdo em TVs corporativas, com gerenciamento centralizado e execução local inteligente.

---

# 🧠 Visão Geral

O sistema é dividido em dois componentes principais:

## 📺 AdLumeClient
Aplicação que roda nas TVs (ou dispositivos conectados), responsável por:
- baixar conteúdos automaticamente
- armazenar localmente (cache)
- executar playlists
- respeitar horários definidos
- rodar de forma resiliente (offline-first)

## 🧩 AdLumeDash
Backend responsável por:
- gerenciar mídias
- definir playlists
- configurar agendamentos
- distribuir configurações para os clients
- servir arquivos de mídia

---

# 🏗️ Arquitetura

```
AdLumeDash (API)
     ↓
 /device/{id}/config
     ↓
AdLumeClient
     ↓
Download mídia (/media/{file})
     ↓
Execução via mpv.exe
```

---

# 🚀 Tecnologias

### Backend (AdLumeDash)
- .NET Web API
- C#
- (futuro) SQL Server / PostgreSQL
- Storage local (evoluindo para S3)

### Client (AdLumeClient)
- .NET
- Player externo: mpv.exe
- Cache local de mídia

---

# 📡 API Endpoints

## 🔹 Configuração do Device
GET /device/{deviceId}/config

### Exemplo de resposta:
```json
{
  "version": 1,
  "media": [
    {
      "hash": "a",
      "url": "http://localhost:5080/media/a.mp4",
      "type": "video"
    }
  ],
  "playlists": [
    {
      "id": "playlist-1",
      "items": [
        {
          "mediaHash": "a",
          "duration": 0
        }
      ]
    }
  ],
  "schedule": [
    {
      "playlistId": "playlist-1",
      "start": "00:00",
      "end": "23:59",
      "days": ["Mon","Tue","Wed","Thu","Fri","Sat","Sun"]
    }
  ]
}
```

---

## 🎥 Download de Mídia
GET /media/{fileName}

Exemplo:
http://localhost:5080/media/a.mp4

---

# 📁 Estrutura do Projeto

```
AdLumeDash/
│
├── Controllers/
│   ├── ConfigController.cs
│   └── MediaController.cs
│
├── Models/
├── Services/
├── Repositories/
├── DTOs/
├── Storage/
│
└── Program.cs
```

---

# ⚙️ Setup Inicial

## 1. Clonar o projeto
```bash
git clone https://github.com/seu-usuario/adlume.git
cd adlume/AdLumeDash
```

## 2. Rodar a API
```bash
dotnet run
```

## 3. Acessar
http://localhost:xxxx/device/{guid}/config

---

# 📦 Armazenamento de Mídia

Arquivos são armazenados localmente em:

/storage/{fileName}

Exemplo:
storage/a.mp4

---

# 🔄 Fluxo do Client

1. Faz polling no /config
2. Verifica versão
3. Baixa mídias novas
4. Remove mídias antigas
5. Monta playlist
6. Executa via mpv

---

# 🧠 Conceitos-Chave

- Offline-first
- Cache por hash
- Config versionada
- Execução desacoplada

---

# 🚧 Roadmap

## 🔹 Em andamento
- [x] API base
- [x] Endpoint de config
- [x] Download de mídia
- [x] Integração com player

## 🔹 Próximos passos
- [ ] Upload com hash (SHA-256)
- [ ] Persistência em banco
- [ ] Sincronização inteligente
- [ ] Engine de schedule completo
- [ ] Monitoramento de devices

---

# 🎯 Objetivo do Projeto

Criar uma solução simples, robusta e escalável para:

Transformar qualquer TV em um canal automatizado de comunicação e monetização.

---

# 👨‍💻 Autor

Carlos Buosi  
BBS Informática

---

# 📜 Licença

Uso privado/comercial (definir futuramente)
