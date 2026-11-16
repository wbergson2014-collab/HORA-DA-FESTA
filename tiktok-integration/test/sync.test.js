import {test} from 'node:test';
import assert from 'node:assert/strict';
import {spawn} from 'node:child_process';
import {readFile} from 'node:fs/promises';
import {createServer} from 'node:http';
import vm from 'node:vm';
import {once} from 'node:events';
import {WebSocket} from 'ws';
const pause=ms=>new Promise(r=>setTimeout(r,ms));
const wait=async check=>{for(let i=0;i<100;i++){if(check())return;await pause(30)}throw Error('Timeout')};
function panelHarness(){const elements=new Map();const el=id=>{if(!elements.has(id))elements.set(id,{value:'',textContent:'',className:'',dataset:{},append(){},replaceChildren(){},click(){}});return elements.get(id)};el('fireworksSeconds').value='10';el('danceSeconds').value='30';el('roseQty').value='15';el('giftCoins').value='20';let data;class Socket {static OPEN=1;static CONNECTING=0;constructor(){this.readyState=1;this.sent=[];Socket.last=this}send(s){this.sent.push(JSON.parse(s))}close(){}}
 const context=vm.createContext({console,WebSocket:Socket,document:{getElementById:el,querySelectorAll:()=>[],createElement:()=>({append(){}})},localStorage:{getItem:()=>data,setItem:(k,v)=>data=v},setTimeout:()=>0,clearTimeout(){},setInterval(){},Date,Math,JSON,Blob,URL,confirm:()=>true});return {context,el,Socket};}
test('panel publishes actual rules, replacement, effects and reconnect state',async()=>{
 const html=await readFile(new URL('../../painel-admin/index.html',import.meta.url),'utf8');const h=panelHarness();vm.runInContext(html.match(/<script>([\s\S]*?)<\/script>/)[1],h.context);const evaluate=s=>vm.runInContext(s,h.context);h.Socket.last.onopen();h.el('viewer').value='alice';h.el('enter').onclick();assert.equal(h.Socket.last.sent.at(-1).state.people[0].name,'alice');
 h.el('mode2').onclick();h.el('viewer').value='blocked';h.el('enter').onclick();assert.equal(evaluate('Object.keys(state.people).length'),1);
 evaluate('roses("alice",14);roses("alice",1);render()');assert.match(evaluate('state.people.alice.role'),/noivo|noiva/);assert.equal(evaluate('state.people.alice.roses'),0);
 for(let i=0;i<7;i++)evaluate('promoteBand("m'+i+'");render()');assert.equal(evaluate('state.band.length'),6);assert.equal(evaluate('state.people.m0.role'),'quadrilha');assert.equal(evaluate('new Set(state.band.map(m=>m.instrument)).size'),6);
 evaluate('promoteCouple("m1");render()');assert.equal(evaluate('state.band.length'),5);evaluate('promoteBand("m1");render()');assert.equal(evaluate('Object.values(state.couple).includes("m1")'),false);
 h.el('viewer').value='effects';evaluate('gift("perfume")');assert.equal(h.Socket.last.sent.at(-1).state.effect.kind,'fogos');assert.equal(evaluate('state.people.effects.role'),'quadrilha');evaluate('gift("donut")');assert.equal(h.Socket.last.sent.at(-1).state.effect.kind,'quadrilha');
 const count=evaluate('Object.keys(state.people).length');h.Socket.last.onopen();assert.equal(h.Socket.last.sent.at(-1).state.people.length,count);
 h.Socket.last.onmessage({data:JSON.stringify({type:'tiktok-event',kind:'gift',user:'live',gift:'heart'})});assert.equal(h.Socket.last.sent.at(-1).state.people.find(p=>p.name==='live').role,'banda');
 h.el('reset').onclick();assert.equal(h.Socket.last.sent.at(-1).state.people.length,0);
});
test('real WebSockets deliver state, replay, commands and source 8781 contract',async t=>{
 let cursor=0;const source=createServer((req,res)=>{assert.ok(req.url.startsWith('/roblox/events'));res.setHeader('content-type','application/json');res.end(JSON.stringify({live:true,username:'tester',session:'test',cursor,events:cursor?[{type:'gift',uniqueId:'live',giftName:'Perfume',count:1,diamondCount:100}]:[]}))});source.listen(0,'127.0.0.1');await once(source,'listening');t.after(()=>source.close());
 const port=19000+Math.floor(Math.random()*10000);const child=spawn(process.execPath,['server.js'],{cwd:new URL('..',import.meta.url),env:{...process.env,PORT:String(port),SOURCE_URL:'http://127.0.0.1:'+source.address().port},stdio:['ignore','pipe','pipe']});t.after(()=>child.kill());await once(child.stdout,'data');
 const connect=async()=>{const ws=new WebSocket('ws://127.0.0.1:'+port);const messages=[];ws.on('message',s=>messages.push(JSON.parse(String(s))));await once(ws,'open');t.after(()=>ws.terminate());return {ws,messages}};
 const panel=await connect(),unity=await connect();unity.ws.send(JSON.stringify({action:'unity-register'}));
 const state={mode:1,people:[{name:'alice',role:'quadrilha',roses:0}],band:[],couple:{noivo:null,noiva:null},camera:'wide',effect:null};panel.ws.send(JSON.stringify({action:'panel-state',state}));await wait(()=>unity.messages.some(m=>m.type==='game-state'));assert.deepEqual(unity.messages.find(m=>m.type==='game-state').state,state);
 const reconnect=await connect();reconnect.ws.send(JSON.stringify({action:'unity-register'}));await wait(()=>reconnect.messages.some(m=>m.type==='game-state'));assert.deepEqual(reconnect.messages.find(m=>m.type==='game-state').state,state);
 panel.ws.send(JSON.stringify({action:'game-command',command:'camera',payload:{mode:'band'}}));await wait(()=>unity.messages.some(m=>m.type==='game-command'));
 panel.ws.send(JSON.stringify({action:'tiktok-connect',username:'tester'}));await pause(150);cursor=1;await wait(()=>unity.messages.some(m=>m.type==='tiktok-event'));assert.equal(unity.messages.find(m=>m.type==='tiktok-event').gift,'perfume');assert.ok(panel.messages.some(m=>m.type==='tiktok-event'));
 unity.ws.send(JSON.stringify({action:'panel-state',state:{...state,people:[]}}));panel.ws.send(JSON.stringify({action:'panel-state',state:{...state,band:Array(7).fill({})}}));await pause(100);assert.equal(unity.messages.filter(m=>m.type==='game-state').length,1);
});
