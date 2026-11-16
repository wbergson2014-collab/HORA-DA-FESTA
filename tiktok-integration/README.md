# Ponte local HORA DA FESTA

Requer Node.js 24 LTS. Execute npm ci, npm test e npm start nesta pasta. WebSocket em ws://127.0.0.1:8765. Reutiliza o coletor LIVE existente em http://127.0.0.1:8781/roblox/events, sem abrir outra conexão TikTok. Não armazene credenciais no repositório.

Mantenha uma aba de painel-admin/index.html aberta: as regras são aplicadas nela e o estado completo é publicado ao Unity. Sem LIVE, todos os controles de simulação continuam disponíveis.

Consulte [contrato](../docs/CONTRATO-UNITY.md) e [teste/sincronização](../docs/UNITY-SINCRONIZACAO.md).
