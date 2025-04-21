# Mongo Analytics - 유저 로그 분석 콘솔 툴

Mongo Analytics는 MongoDB를 기반으로 유저 로그 데이터를 분석하고,  
콘솔 환경에서 다양한 통계 결과를 출력하는 분석 도구입니다.  
유저 이탈 시점 분석, 퍼널(Funnel) 분석, 구간별 행동 횟수 등  
라이브 게임/서비스 운영에 필요한 기초 통계를 CLI 환경에서 바로 확인할 수 있습니다.

## 주요 기술 스택

- C# (.NET 8)
- MongoDB Driver
- LINQ 기반 Query 처리
- Console 기반 결과 출력

## 주요 기능

### ✅ 유저 로그 조회
- 특정 유저 ID 또는 조건에 맞는 로그 필터링
- 콘솔에 목록 형태로 출력

### ✅ 퍼널 분석 (Funnel Analysis)
- 주요 행동 구간별 유저 이탈률 분석
- 예: 스테이지 진입 → 클리어 → 다음 스테이지 전환률

### ✅ 구간별 카운팅
- 행동 유형별 발생 횟수 (예: 로그인, 결제, 스테이지 실패, 아이템 사용 등)
- MongoDB Aggregation Pipeline을 활용한 통계 계산

## 실행 방법

```bash
# MongoDB 서버 접속 필요
# 환경 설정은 appsettings.json 또는 내부 설정 클래스에서 구성

dotnet run
