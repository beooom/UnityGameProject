---
description: Resume or start interview prep workspace — lists existing companies if no arg, resumes specific company workspace if name given, or starts new prep if URL/text provided
allowed-tools: Read, Bash, Glob, Grep, WebFetch, Task
argument-hint: (optional) company name | URL | "new"
---

# /interview — 면접 준비 세션 자동 시작

사용자가 `/interview` 또는 `/interview <인자>` 를 입력하면 인자에 따라 분기:

## 분기 1: 인자 없음 → 기존 회사 목록 + 선택

```
1. Bash: ls -1 Docs/InterviewPrep/companies/ (디렉토리 목록)
2. 각 회사 폴더에서 00-context.md 의 메타 (포지션 + 면접 일정) 추출
3. 표 형태로 사용자에게 보여줌:

   | # | 회사 | 포지션 | 면접 일정 | 마지막 활동 |
   |---|------|-------|----------|------------|
   | 1 | 111percent | 멀티플레이 엔진 | 2026-04-14 (완료) | <git 마지막 커밋> |
   | 2 | devcat | 게임 프레임워크 | 미정 | <...> |

4. 사용자에게: "어느 회사 진행할래? 번호 또는 이름. 새 회사면 모집요강 URL/텍스트 줘."
```

## 분기 2: 인자가 기존 회사명 → 워크스페이스 재개

```
1. Bash: ls Docs/InterviewPrep/companies/<arg>/ (존재 확인)
2. 존재하면:
   - Read 00-context.md (튜터 모드 + 학습 루틴 진행 상황)
   - Read 01-company-research.md 첫 30줄 (회사 컨텍스트 리마인드)
   - Read 03-expected-questions.md 의 자신감 분포
   - Read 04-mock-interview.md 마지막 회차 (있으면)
   - Bash: git log --oneline -5 -- Docs/InterviewPrep/companies/<arg>/ (이 회사 최근 활동)
3. 컨텍스트 패키징 후 interview-prep-builder 호출 (아래 분기 4 참고)
```

## 분기 3: 인자가 URL 또는 긴 텍스트 → 새 회사 시작

```
1. URL이면 WebFetch 로 모집요강 가져옴
2. 회사명 추출 시도 (모집요강 헤더 또는 도메인)
3. 사용자 확인: "회사명이 X 맞아? 폴더명으로 쓸 영문 소문자 형태로 알려줘."
4. 확인 후 interview-prep-builder 호출 (Phase 0 부터)
```

## 분기 4: 인자가 "new" → 사용자에게 모집요강 요청

```
1. "모집요강 URL 또는 텍스트 줘. 회사명도 함께."
2. 입력 받으면 분기 3 처럼 진행
```

## 컨텍스트 패키징 (interview-prep-builder 에 넘김)

분기에 따라 패키징 내용 다름:

### 새 회사 (분기 3, 4)
```
모집요강 (텍스트 또는 URL):
<...>

회사명: <영문 소문자>
오늘 날짜: <YYYY-MM-DD>
사용자 프로파일: 참고 — Docs/TurnIn/me/, .claude/agents/roadmap-tutor.md 의 사용자 프로파일

지시: Phase 0 (폴더 + 템플릿 복사) 부터 시작. Phase 1 (회사 조사) 진행 후 사용자 검토 받고 다음 Phase. 한 번에 6개 다 만들지 말 것.
```

### 기존 회사 재개 (분기 2)
```
회사: <name>
오늘 날짜: <YYYY-MM-DD>
면접 일정: <00-context.md 에서>
D-day: <남은 일수>

마지막 활동 (git):
<git log 결과>

워크스페이스 진행 상태:
- 00-context.md: 작성됨/미작성
- 01-company-research.md: 작성됨/미작성
- 02-tech-deep-dive.md: 작성됨/미작성
- 03-expected-questions.md: 작성됨/미작성, 자신감 분포 (🟢 N / 🟡 M / 🔴 K)
- 04-mock-interview.md: 모의 N회차 진행됨
- 05-tutor-sessions.md: M개 세션

가장 최근 식별된 약점:
<03 의 🔴 항목 또는 04 의 마지막 정정 항목>

지시: 첫 메시지로 "지난번 X 까지 했고, 오늘 D-day Y 일 남았으니 Z 하자" 한 단락 요약 + 진단 질문 1개. 사용자 답에 따라 모드 분기:
- "모의면접 ㄱㄱ" → 04 회차 추가 모드
- "약점 채우기" → 🔴 항목 중 하나 골라 깊이 파기
- 사용자 명시 주제 → 그쪽
```

## 분기 별 사용 예시

### 예시 1: 기존 워크스페이스 재개

```
사용자: /interview 111percent

→ 자동으로:
  - 워크스페이스 발견
  - 마지막 활동: edf975a (워크스페이스 이동, 2026-04-15)
  - 실면접 완료 (2026-04-14), 결과 대기 중
  - 04-mock 의 마지막 회차: 실면접 복기 (틀린 답변 4개)

→ builder 첫 메시지:
  "111percent 면접은 끝났어 (어제). 결과 대기 중이고. 복기 자료 보니 GC 세대 / BlobData / Allocator / 캐시 4개를 정정해야 해. 
   이건 회사 무관 일반 지식이라 Foundations/CSharp + ComputerArchitecture 로 가야 함. 
   그 보강 진행할까, 아니면 다음 회사 준비할까?"
```

### 예시 2: 새 회사 시작

```
사용자: /interview https://career.nexon.com/posting/12345

→ 자동으로:
  - WebFetch 로 모집요강 가져옴
  - 회사: nexon, 포지션: 클라이언트 엔진 개발자
  - 사용자 확인: "회사명 'nexon' OK?"

사용자: OK

→ builder Phase 0:
  - mkdir Docs/InterviewPrep/companies/nexon
  - 6개 템플릿 복사
  
→ builder Phase 1:
  - WebSearch 병렬: 넥슨 기술블로그, 면접후기, GitHub
  - 01-company-research.md 채움
  - "01 채워졌어. 검토 후 OK 하면 Phase 2 (튜터 모드 + 강점 매핑) 갈게."
```

### 예시 3: 인자 없음 (목록)

```
사용자: /interview

→ 표 보여줌 + "어느 회사?"
사용자: 1

→ 분기 2 진입 (111percent 재개)
```

## 절대 하지 말 것

- ❌ 인자 없을 때 "모집요강 줘" 부터 묻기 — 먼저 기존 목록 보여줌
- ❌ 회사명 자동 추출이 애매한데 그냥 진행 — 사용자 확인 필수
- ❌ Phase 1~5 한꺼번에 dump — Phase 별 검토 받음
- ❌ 일반 이론을 회사 폴더에 작성 — Foundations/ 링크만

## 성공 기준

`/interview` 한 번 입력 → **30초 안에** "어느 회사 / 모집요강 / 신규?" 분기 결정 → 다음 단계 명확. 사용자가 컨텍스트 다시 설명할 필요 없음.
