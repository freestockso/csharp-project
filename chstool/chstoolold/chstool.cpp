// 这是主 DLL 文件。

#include "stdafx.h"

#include "chstool.h"
#include <string> 
#include <map>
#include <set>
#include <sstream>
#include <time.h>

using namespace std;

#ifdef _MANAGED
#pragma managed(push, off)
#endif

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		break;
	case DLL_THREAD_ATTACH:
		 break;
	case DLL_THREAD_DETACH:
		 break;
	case DLL_PROCESS_DETACH:
		break;
	}
    return TRUE;
}

#ifdef _MANAGED
#pragma managed(pop)
#endif

string buildkey(string stklabel,int no){
	stringstream ss;
	ss<<no;
	string key=stklabel+'-'+ss.str();
	return key;
}

map<int,float> globalvars;//全局值
map<string,float> stockvars;//某只股票的值
map<int,float> statistics;//存储统计值
map<int,set<string>> statispool;//用于统计时记录股票
map<string,time_t> begintimes;//记录要延时计算的起始时间
map<string,int> stockcounts;

__declspec(dllexport) int WINAPI SelfCount(CALCINFO* pData){
	if(pData->m_pfParam1){
		string stklabel=pData->m_strStkLabel;
		int no;
		if (pData->m_pfParam2){
			no=(int)*pData->m_pfParam2;
		}else {
			no=0;
		}
		string key=buildkey(stklabel,no);
		float value;
		int pos=pData->m_nNumData-1;
		if (pData->m_nParam1Start>=0){//参数1为序列
			value=pData->m_pfParam1[pos];//只取序列中的最后一个数据
		}else {//参数1为常数
			value=*pData->m_pfParam1;
		}
		if (stockcounts.count(key)<=0){
			stockcounts[key]=0;
		}
		if (value>0){
			stockcounts[key]++;
		}
		pData->m_pResultBuf[pos]=(float)stockcounts[key];
		return pos;
	}
	return -1;
}
__declspec(dllexport) int WINAPI Delay(CALCINFO* pData){
	if(pData->m_pfParam1 && pData->m_pfParam2){
		string stklabel=pData->m_strStkLabel;
		int minutes=(int)*pData->m_pfParam2;
		int no;
		if (pData->m_pfParam3){
			no=(int)*pData->m_pfParam3;
		}else {
			no=0;
		}
		string key=buildkey(stklabel,no);
		time_t now=time(NULL);
		if (begintimes.count(key)>0) {
			if (now-begintimes[key]>=minutes*60){
				int pos=pData->m_nNumData-1;
				if (pData->m_nParam1Start>=0){//参数1为序列
					pData->m_pResultBuf[pos]=pData->m_pfParam1[pos];//只存储序列中的最后一个数据
				}else {//参数1为常数
					pData->m_pResultBuf[pos]=*pData->m_pfParam1;
				}
				map<string,time_t>::iterator it;
				it=begintimes.find(key);
				if (it!=begintimes.end()) begintimes.erase(it);
				return pos;
			}
		}else {
			begintimes[key]=now;
		}
	}
	return -1;
}
__declspec(dllexport) int WINAPI TransStatis(CALCINFO* pData){
	if(pData->m_pfParam1 && pData->m_pfParam2){
		int key=(int)*pData->m_pfParam2;
		set<string> pool=statispool[key];
		string stklabel=pData->m_strStkLabel;
		if (pool.count(stklabel)>0){//如果已经存在此股票代码，则说明已经统计过一次
			statistics[key]=0;
			pool.clear();
		}

		pool.insert(stklabel);
		statispool[key]=pool;
		float value;
		if (pData->m_nParam1Start>=0){//参数1为序列
			int pos=pData->m_nNumData-1;
			value=pData->m_pfParam1[pos];//只存储序列中的最后一个数据
		}else {//参数1为常数
			value=*pData->m_pfParam1;
		}
		statistics[key]+=value;
	}
	return -1;
}

__declspec(dllexport) int WINAPI TransStatisValue(CALCINFO* pData){
	if(pData->m_pfParam1 && pData->m_nParam1Start<0){
		int key=(int)*pData->m_pfParam1;
		float value=statistics[key];
		int pos=pData->m_nNumData-1;
		pData->m_pResultBuf[pos]=value;//只存入返回数据在最后一个位置
		return pos;//返回数据的起始位置
	}else {
		return -1;
	}
}
__declspec(dllexport) int WINAPI Write(CALCINFO* pData)
{
	if(pData->m_pfParam1 && pData->m_pfParam2){
		int key=(int)*pData->m_pfParam2;
		float value;
		if (pData->m_nParam1Start>=0){//参数1为序列
			int pos=pData->m_nNumData-1;
			value=pData->m_pfParam1[pos];//只存储序列中的最后一个数据
		}else {//参数1为常数
			value=*pData->m_pfParam1;
		}
		globalvars[key]=value;
	}
	return -1;
}
__declspec(dllexport) int WINAPI Read(CALCINFO* pData)
{
	if(pData->m_pfParam1 &&					//参数1有效
		pData->m_nParam1Start<0 ){//参数1为常数
			int key=(int)*pData->m_pfParam1;
			float value=globalvars[key];
			/*for(int i=0;i<pData->m_nNumData;i++){
				pData->m_pResultBuf[i]=value;
			}
			return 0;*/
			int pos=pData->m_nNumData-1;
			pData->m_pResultBuf[pos]=value;//只存入返回数据在最后一个位置
			return pos;//返回数据的起始位置
	}else {
		return -1;
	}
}
__declspec(dllexport) int WINAPI SelfWrite(CALCINFO* pData)
{
	if(pData->m_pfParam1 &&	pData->m_pfParam2){
		string stklabel=pData->m_strStkLabel;
		int no=(int)*pData->m_pfParam2;
		string key=buildkey(stklabel,no);
		float value;
		if (pData->m_nParam1Start>=0){//参数1为序列
			int pos=pData->m_nNumData-1;
			value=pData->m_pfParam1[pos];//只存储序列中的最后一个数据
		}else {//参数1为常数
			value=*pData->m_pfParam1;
		}
		stockvars[key]=value;
	}
	return -1;
}
__declspec(dllexport) int WINAPI SelfRead(CALCINFO* pData)
{
	if(pData->m_pfParam1 &&					//参数1有效
		pData->m_nParam1Start<0 ){//参数1为常数
			string stklabel=pData->m_strStkLabel;
			int no=(int)*pData->m_pfParam1;
			string key=buildkey(stklabel,no);
			float value=stockvars[key];
			/*for(int i=0;i<pData->m_nNumData;i++){
				pData->m_pResultBuf[i]=value;
			}
			return 0;*/
			int pos=pData->m_nNumData-1;
			pData->m_pResultBuf[pos]=value;//只存入返回数据在最后一个位置
			return pos;//返回数据的起始位置
	}else {
		return -1;
	}
}
