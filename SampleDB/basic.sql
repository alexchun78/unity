
  /******  Comment  ******/ 
  -- 1 each line comment
  -- SQL (RDBMS를 조작하기 위한 명령어)
  -- +@ T-SQL 
  -- CRUD ( Create - Read - Update - Delete)

  /*************************************************/
  /******  SELECT ******/  
  /******  NULL 비교 구문  ******/ 
  -- SELECT nameFirst,nameLast,birthYear, birthCountry, weight FROM players WHERE weight IS NULL
  -- =, != 사용이 안된다.
  -- IS, IS NOT 을 사용한다.
  -- NULL은 값 자체가 없는 상태여서 비교 연산자는 사용 못한다.

  /******  LIKE 사용법  ******/ 
  -- SELECT * FROM players WHERE birthCity LIKE 'New%';
  -- % 임의의 문자열
  -- _ 임의의 문자 1개 
  /************************************************/

  /*
  SELECT TOP 10 * 
  FROM players 
  WHERE birthYear IS NOT NULL 
  ORDER BY birthYear DESC, birthMonth DESC, birthDay DESC;
  -- ORDER BY 
  -- ASC : 오름차순
  -- DESC : 내림차순
  -- TOP 숫자 * : 전체 중 상위 몇 명 만
  -- TOP 숫자 PERCENT * : 상위 몇 퍼센트 만  
  
  SELECT * 
  FROM players 
  WHERE birthYear IS NOT NULL 
  ORDER BY birthYear DESC, birthMonth DESC, birthDay DESC
  OFFSET 100 ROWS FETCH NEXT 100 ROWS ONLY;
  
  -- 100 ~ 200 사이 추출 : OFFSET 100 ROWS FETCH NEXT 100 ROWS ONLY, TOP와는 함께 사용 불가하다.
  */
  /*************************************************/