// DzhCLI.cpp : 定义 DLL 应用程序的入口点。
//

#include "stdafx.h"
#include "DzhCLI.h"
using namespace DzhSample;

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
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
    return TRUE;
}

#ifdef _MANAGED
#pragma managed(pop)
#endif
//另一个C# DLL(DzhSample.DLL)里有什么输出函数,这里就原样输出一个
//建议使用同名函数
//所有输出函数,记得在DzhCLI.h里添加定义,如:__declspec(dllexport) int WINAPI SHOWFORMVAR(CALCINFO* pData);
////////////
//计算平均值.
//大智慧公式语法:p1:= "DzhCLI @ AVERAGEVAR"(5);//5日平均
__declspec(dllexport) int WINAPI AVERAGEVAR(CALCINFO* pData)
{
	CDzhSample pDzhSample;
	int nResult = pDzhSample.AVERAGEVAR((int)(LONG_PTR)pData);
	return nResult;
}
////////////
//计算平均值差.
//大智慧公式语法:p1:= "DzhCLI @ AVERAGEDIFFVAR"(5,12);//5日平均 - 12日平均
__declspec(dllexport) int WINAPI AVERAGEDIFFVAR(CALCINFO* pData)
{
	CDzhSample pDzhSample;
	int nResult = pDzhSample.AVERAGEDIFFVAR((int)(LONG_PTR)pData);
	return nResult;
}
////////////
//计算MACD.
//大智慧公式语法:p1:= "DzhCLI @ MACDVAR"(12,26,9);//12日平均 - 26日平均,对差值再做成9天平均,再做差
__declspec(dllexport) int WINAPI MACDVAR(CALCINFO* pData)
{
	CDzhSample pDzhSample;
	int nResult = pDzhSample.MACDVAR((int)(LONG_PTR)pData);
	return nResult;
}
////////////
//显示一个窗口.
//大智慧公式语法:p1:= "DzhCLI @ SHOWFORMVAR";
__declspec(dllexport) int WINAPI SHOWFORMVAR(CALCINFO* pData)
{
	CDzhSample pDzhSample;
	int nResult = pDzhSample.SHOWFORMVAR((int)(LONG_PTR)pData);
	return nResult;
}
