# SSMS 2020으로 업데이트..
 기존 2018은 git이 꼬인건지.. 변환 후 연결이 안되서 삭제후 다시 올림. 

 - 오브젝트 찾기에서 Control + C 안되는 현상에 대하여 조치방법.
   도구 / 옵션 / 환경 / 문서로 이동 
			     '솔루션 탐색기에 기타 파일 표시'(첨부 참조)를 선택 취소
			하고 SSMS를 다시 시작하십시오. 

- SSMS 위치 지정. ( 기존 빌드 이벤트 삭제 )
![image](https://github.com/user-attachments/assets/9bcac9ba-956e-485d-84eb-a5efed0a78ac)

- 오류 발견 : 실행중 도킹창을 드래그 드랍하여 프로그램에 도킹할때 강제종료됨.
		   원인 : ssms 18.3으로 실행했을때 에러 - 18.5에서 개선되었다는 글을 보고 최신버젼으로 재설치
				  18.8버젼이라서 닷넷버젼을 4.72로 변경 후 재컴파일

- SSMS의 2020 버젼으로 업그레이드.
		1. 빌드에 [VSIX]탭에서 Copy VSIX Content to the following location: 에 SSMS 2020이 설치된 확장 디렉토리에 "SSMS2020" 디렉토리 지정!! 
		그러면 자동으로 복사된다 (빌드 이벤트 안써도 된다)
		예) I:\SSMS20\Common7\IDE\Extensions\ssms2020
		2. 참조경로 추가 
			I:\SSMS20\Common7\IDE\
			I:\SSMS20\Common7\IDE\Extensions\Application\
		3. SkipLoading.2020.reg 레지스트리 파일 추가
			
			Windows Registry Editor Version 5.00
			[HKEY_CURRENT_USER\Software\Microsoft\SQL Server Management Studio\20.0_IsoShell\Packages\{f1536ef8-92ec-443c-9ed7-fdadf150da82}]
			"SkipLoading"=dword:00000001

			** 여기서 {f1536ef8-92ec-443c-9ed7-fdadf150da82}는 빌드를 하면 VSIX 배포된 곳에서 "JSFW.SSMS.Extensibility.pkgdef" 파일이 있다. 
			   이 파일을 열어보면 
			   "CodeBase"="$PackageFolder$\JSFW.SSMS.Extensibility.dll"
				[$RootKey$\AutoLoadPackages\{f1536ef8-92ec-443c-9ed7-fdadf150da82}]
				"{e499b659-abb0-4651-a054-3deb4f5b6541}"=dword:00000002
			  이런 식으로 써있다. AutoLoadPackages 다음 GUID를 복사해서 사용하니 ssms에서도 올라오더라. ( 임의로 넣었더니 기존과 다르게 안올라왔다. )



   

# JSFW.SSMS.Extensibility
SSMS 2018 확장 (( 💙 내가 가장 애정을 가진 프로그램 중 하나! 💙 ))

목적 : SSMS 에 쿼리를 좀 더 쉽게 작성 할 수 없는가?<br />
 VS처럼 확장프로그램을 넣을순 없나? 에서 시작.<br />
 삽질의 연속으로 만들어진 프로그램이다.<br />
 
```diff
+ 구글링으로 많은 요소들을 찾아 헤매서 취합해서 어디서 어떤 코드를 가져왔는지 명확하지 않다. 
+ 다만, 찾아온 코드들의 상단에는 해당 링크를 기본적으로 붙여놨는데... 
+ 시간이 오래되어 없을 가능성도 있다.
```

```diff
- ## VS를 관리자모드로 실행하여 솔루션을 오픈한다.

- SkipLoading.2018.reg 레지스트리 등록 후 디버깅 시작!!

- 빌드 후 이벤트 명령줄  SSMS의 2018 폴더에 직접 복사 ( 소스 수정은 SSMS를 끄고 한다 )
echo xcopy "$(TargetPath)" "C:\Program Files (x86)\Microsoft SQL Server Management Studio 18\Common7\IDE\Extensions\ssms2018\$(TargetFileName)" /y /r

- 빌드 경로 : ssms2018 
 :: 최초 개발시 폴더 확인 후 해당 폴더파일을 붙여넣기 한다.
```

- 기능
1. 오브젝트 찾기! ( CTRL + Q + F )<br />
   테이블, 뷰, 프로시저, 함수등... <br />
2. 쿼리 저장소<br />
3. 결과 테이블의 틀고정<br />
4. 컬럼 찾기<br />

<br />

- 오브젝트 찾기<br />
![image](https://user-images.githubusercontent.com/116536524/198232613-c652e985-d581-42d8-a6b0-898d8bf3dbab.png)

- 쿼리 저장소 ( vs 의 코드 변환 템플릿과 동일함 )<br />
![image](https://user-images.githubusercontent.com/116536524/198232678-617e46dd-2c34-48de-bfa9-9608e7c3af80.png)

- 컨텍스트 메뉴<br />
![image](https://user-images.githubusercontent.com/116536524/198233002-afa5d934-8ee2-4062-b9c8-751e311d5002.png)

- 틀고정 ( C컬럼이 고정되어 스크롤 하여도 ABC는 움직이지 않는다. )<br />
![image](https://user-images.githubusercontent.com/116536524/198234530-b1f6da43-c3ff-4a91-9c0c-d2778748c15c.png)

- 컬럼찾기<br />
![image](https://user-images.githubusercontent.com/116536524/198233501-2b6964be-6373-4a97-a8c9-0fbeb0c7fd04.png)

- 컬럼찾기 결과 ( H 컬럼을 찾고 H에 배경색을 넣고 포커스 이동 )<br />
![image](https://user-images.githubusercontent.com/116536524/198233607-e4932af1-270d-4cd2-aeaa-ab1b62468e7e.png)


<br />
소스내에서 텍스트박스 ( MIT )<br />
[ICSharpCode.AvalonEdit](https://github.com/icsharpcode/AvalonEdit) <br />


