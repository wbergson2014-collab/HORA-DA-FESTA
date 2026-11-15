# Integração local TikTok LIVE / Unity

## Instalar e iniciar
No PowerShell, na pasta `tiktok-integration`:

```powershell
npm install
npm start
```

Requer Node.js 20+ e internet. O painel HTML conecta automaticamente a `ws://127.0.0.1:8765`. Digite o usuário TikTok sem @ e clique em **Conectar TikTok**. A conta deve estar em LIVE; o TikTok pode alterar suas APIs e a conexão pode falhar. A biblioteca de terceiros não é uma API oficial do TikTok.

## Unity
O jogo ainda precisa implementar um cliente WebSocket que conecte a `ws://127.0.0.1:8765` e envie `{"action":"unity-register"}` após a conexão. O status ficará verde somente enquanto o cliente estiver conectado. O protocolo `game-command` é reservado para futura integração; **a cena 3D não é controlada ainda**.

## Segurança
O servidor escuta apenas 127.0.0.1. Não exponha a porta 8765 à internet. O painel não tem autenticação: use somente no computador local. O repositório público não deve conter tokens nem credenciais.

## Limitações
A conexão TikTok e a identificação de presentes dependem dos nomes que a plataforma enviar, que podem variar conforme idioma e catálogo. A contagem de presentes em sequência precisa ser validada em LIVE real. A entrada `!entrar` respeita o modo do painel. Ainda falta enviar os eventos efetivos do painel para o Unity; o bridge já suporta clientes Unity, mas não implementa lógica de gameplay.
