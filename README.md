# SSMS 2020으로 업데이트..
 기존 2018은 git이 꼬인건지.. 변환 후 연결이 안되서 삭제후 다시 올림. 

 . 오브젝트 찾기에서 Control + C 안되는 현상에 대하여 조치방법.
   도구 / 옵션 / 환경 / 문서로 이동 
			     '솔루션 탐색기에 기타 파일 표시'(첨부 참조)를 선택 취소
			하고 SSMS를 다시 시작하십시오. 

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


