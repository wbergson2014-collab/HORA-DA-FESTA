import { WebSocketServer, WebSocket } from "ws";
import { TikTokLiveConnection, WebcastEvent } from "tiktok-live-connector";
const PORT=8765;
const wss=new WebSocketServer({host:"127.0.0.1",port:PORT});
const panels=new Set(),unity=new Set();
let tiktok=null,username="",ttState="disconnected",generation=0;
function status(){return {type:"status",tiktok:ttState,unity:unity.size?"connected":"disconnected",username}}
function send(ws,obj){if(ws.readyState===WebSocket.OPEN)ws.send(JSON.stringify(obj))}
function broadcast(obj){for(const ws of panels)send(ws,obj)}
function sync(){broadcast(status())}
function notice(message){broadcast({type:"notice",message});console.log(message)}
async function disconnectTikTok(){generation++;const prev=tiktok;tiktok=null;username="";ttState="disconnected";if(prev)try{await prev.disconnect()}catch{}sync()}
async function connectTikTok(user){await disconnectTikTok();username=user;const mine=++generation;ttState="connecting";sync();notice("Conectando ao TikTok LIVE de @"+user+"...");const conn=new TikTokLiveConnection(user,{enableExtendedGiftInfo:true});tiktok=conn;
conn.on(WebcastEvent.CHAT,data=>{if(mine!==generation)return;const comment=String(data.comment||"").trim().toLowerCase();if(comment==="!entrar")broadcast({type:"tiktok-event",kind:"join",user:data.uniqueId})});
conn.on(WebcastEvent.GIFT,data=>{if(mine!==generation)return;
// Gifts with repeatEnd=false are intermediate updates; count only the final packet.
if(data.giftType===1&&data.repeatEnd!==true)return;
const name=String(data.giftName||"").toLowerCase();
const gift=name.includes("rosa")||name==="rose"?"rose":name==="gg"?"gg":name.includes("coração com dedos")||name.includes("finger heart")?"heart":name.includes("hora do café")||name.includes("coffee time")?"coffee":name.includes("perfume")?"perfume":name.includes("rosquinha")||name.includes("doughnut")||name.includes("donut")?"donut":"generic";
const count=data.giftType===1?Math.max(1,Number(data.repeatCount)||1):1;
const coins=Number(data.diamondCount)||0;
broadcast({type:"tiktok-event",kind:"gift",user:data.uniqueId,gift,count,coins,name:data.giftName||""});
});
conn.on(WebcastEvent.DISCONNECTED,()=>{if(mine!==generation)return;ttState="disconnected";sync();notice("TikTok desconectado. Use Conectar TikTok para tentar novamente.")});
conn.on(WebcastEvent.ERROR,e=>{if(mine!==generation)return;ttState="error";sync();notice("Erro TikTok: "+String(e?.message||e))});
try{await conn.connect();if(mine!==generation){await conn.disconnect();return}ttState="connected";sync();notice("TikTok LIVE conectado: @"+user)}catch(e){if(mine!==generation)return;ttState="error";sync();notice("Falha ao conectar TikTok: "+String(e?.message||e))}}
wss.on("connection",(ws,req)=>{const origin=req.headers.origin||"";if(origin&&!origin.startsWith("file://")&&!/^https?:\/\/(localhost|127\.0\.0\.1)(:\d+)?$/.test(origin)){ws.close(1008,"Origem nao permitida");return}
let role="panel";panels.add(ws);send(ws,status());ws.on("message",async raw=>{let msg;try{msg=JSON.parse(String(raw))}catch{return}
if(msg.action==="unity-register"){panels.delete(ws);role="unity";unity.add(ws);sync();notice("Jogo Unity conectado.");return}
if(role!=="panel")return;
if(msg.action==="status")send(ws,status());
if(msg.action==="tiktok-connect"&&/^[a-zA-Z0-9._]{2,30}$/.test(String(msg.username||"")))await connectTikTok(msg.username);
if(msg.action==="tiktok-disconnect")await disconnectTikTok();
if(msg.action==="game-command")for(const client of unity)send(client,{type:"game-command",command:msg.command,payload:msg.payload||{}});
});ws.on("close",()=>{panels.delete(ws);if(unity.delete(ws)){sync();notice("Jogo Unity desconectado.")}})});
console.log("HORA DA FESTA bridge: ws://127.0.0.1:"+PORT);
