# 🚀 AdLume

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Status](https://img.shields.io/badge/status-active-success)
![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux-blue)
![License](https://img.shields.io/badge/license-private-red)

Sistema de reprodução de mídia com controle remoto via API, sincronização offline e integração com player leve.

---

## 📖 Visão Geral

O **AdLume** é um sistema projetado para distribuição e execução de playlists de mídia em dispositivos remotos.

- Offline-first  
- Baixo consumo de recursos  
- Controle centralizado via API  
- Execução headless com mpv  

---

## 🧱 Arquitetura

```mermaid
flowchart LR
    A[API .NET] --> B[Cliente AdLume]
    B --> C[Cache Local]
    B --> D[mpv Player IPC]
```

---

## 🧩 Componentes

### 🔹 API (.NET)
- Fornece playlists por dispositivo
- Serve arquivos de mídia
- Protegida via Bearer Token

### 🔹 Cliente

Compatível com **Windows e Linux**, utilizando .NET 8.

- Consome API
- Faz cache local (JSON)
- Sincroniza mídias
- Controla player via socket IPC

### 🌍 Compatibilidade

- Windows
- Linux

Requisitos:
- .NET 8 Runtime
- mpv instalado

---

## 🔐 Autenticação

```http
Authorization: Bearer SEU_TOKEN
```

Validação atual: simples (token fixo).

---

## 🌐 Endpoints

### GET /equipamento/{deviceId}

Retorna playlist.

### GET /media/{nomeMidia}

Retorna arquivo mp4.

---

## 📥 Sync de mídia

- Cria /Videos
- Baixa apenas novos arquivos
- Usa streaming (baixo uso de memória)

---

## 🎥 Player

```bash
mpv --idle=yes --no-terminal --input-ipc-server=/tmp/mpvsocket
```

---

## 🎮 IPC

```json
{ "command": ["loadfile", "video1.mp4", "replace"] }
```

---

## 📡 Fluxo

```mermaid
sequenceDiagram
    participant C as Cliente
    participant A as API
    participant M as mpv

    C->>A: GET playlist
    A-->>C: JSON
    C->>A: download mídia
    C->>M: atualiza playlist
```

---

## 🚀 Roadmap

- JWT completo  
- Retry  
- Paralelismo  
- Hash check  
- Docker  

---

## 📦 Stack

- .NET 8  
- Serilog  
- mpv  

---

## 📄 Licença

Privado.
