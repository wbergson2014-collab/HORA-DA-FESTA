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
// Reuse the already-connected local TikTok collector on port 8781.
const SOURCE="http://127.0.0.1:8781";
let pollTimer=null,sourceSession=null,sourceCursor=null,pollBusy=false;
function stopSource(){if(pollTimer)clearInterval(pollTimer);pollTimer=null;sourceSession=null;sourceCursor=null}
function classifyGift(name){const n=String(name||"").toLowerCase();return n.includes("rosa")||n==="rose"?"rose":n==="gg"?"gg":n.includes("coração com dedos")||n.includes("coracao com dedos")||n.includes("finger heart")?"heart":n.includes("hora do café")||n.includes("hora do cafe")||n.includes("coffee time")?"coffee":n.includes("perfume")?"perfume":n.includes("rosquinha")||n.includes("doughnut")||n.includes("donut")?"donut":"generic"}
function forwardSourceEvent(e){
  const user=String(e.callerHandle||e.uniqueId||e.userId||e.id||"").replace(/^@/,"");
  if(!user)return;
  if(e.type==="gift"){
    broadcast({type:"tiktok-event",kind:"gift",user,gift:classifyGift(e.giftName),count:Math.max(1,Number(e.count)||1),coins:Number(e.diamondCount||e.coins)||0,name:e.giftName||""});
  }else if(e.type==="chat"&&String(e.comment||"").trim().toLowerCase()==="!entrar"){
    broadcast({type:"tiktok-event",kind:"join",user});
  }
}
async function pollSource(mine){
  if(pollBusy||mine!==generation)return;
  pollBusy=true;
  try{
    const query=sourceSession!==null?"?session="+encodeURIComponent(sourceSession)+"&after="+encodeURIComponent(sourceCursor):"";
    const response=await fetch(SOURCE+"/roblox/events"+query,{signal:AbortSignal.timeout(3500)});
    if(!response.ok)throw Error("HTTP "+response.status);
    const data=await response.json();
    if(mine!==generation)return;
    if(!data.live){if(ttState!=="disconnected"){ttState="disconnected";sync();notice("A fonte 8781 nao esta conectada a uma LIVE.")}return}
    if(data.username&&data.username.toLowerCase()!==username.toLowerCase()){ttState="error";sync();notice("A fonte 8781 esta conectada a @"+data.username+", nao a @"+username);return}
    if(data.session!==sourceSession){
      sourceSession=data.session;sourceCursor=Number(data.cursor)||0;
      notice("Fonte 8781 sincronizada. Aguardando novos eventos.");
    }else{
      for(const event of data.events||[])forwardSourceEvent(event);
      sourceCursor=Number(data.cursor)||sourceCursor;
    }
    if(ttState!=="connected"){ttState="connected";sync();notice("TikTok LIVE compartilhado pela porta 8781: @"+username)}
  }catch(e){if(mine!==generation)return;if(ttState!=="error"){ttState="error";sync();notice("Fonte 8781 indisponivel: "+String(e.message||e))}}
  finally{pollBusy=false}
}
async function disconnectTikTok(){generation++;stopSource();username="";ttState="disconnected";sync()}
async function connectTikTok(user){await disconnectTikTok();username=user;const mine=++generation;ttState="connecting";sync();notice("Usando a conexao existente da porta 8781 para @"+user);await pollSource(mine);if(mine===generation)pollTimer=setInterval(()=>pollSource(mine),700)}
wss.on("connection",(ws,req)=>{const origin=req.headers.origin||"";if(origin&&origin!=="null"&&!origin.startsWith("file://")&&!/^https?:\/\/(localhost|127\.0\.0\.1)(:\d+)?$/.test(origin)){ws.close(1008,"Origem nao permitida");return}
let role="panel";panels.add(ws);send(ws,status());ws.on("message",async raw=>{let msg;try{msg=JSON.parse(String(raw))}catch{return}
if(msg.action==="unity-register"){panels.delete(ws);role="unity";unity.add(ws);sync();notice("Jogo Unity conectado.");return}
if(role!=="panel")return;
if(msg.action==="status")send(ws,status());
if(msg.action==="tiktok-connect"&&/^[a-zA-Z0-9._]{2,30}$/.test(String(msg.username||"")))await connectTikTok(msg.username);
if(msg.action==="tiktok-disconnect")await disconnectTikTok();
if(msg.action==="game-command")for(const client of unity)send(client,{type:"game-command",command:msg.command,payload:msg.payload||{}});
});ws.on("close",()=>{panels.delete(ws);if(unity.delete(ws)){sync();notice("Jogo Unity desconectado.")}})});
console.log("HORA DA FESTA bridge: ws://127.0.0.1:"+PORT);
