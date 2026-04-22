---
description: Start a mock interview round for a company workspace — loads context (00-context, 03-expected-questions, 04-mock-interview, portfolio) and spawns the mock-interviewer agent with interviewer persona and mode flag (HR | 기술면접관 | CTO | 자유 | 순차)
allowed-tools: Read, Bash, Glob, Grep, Task
argument-hint: (optional) <company> [mode] [parts|topic]   예) /mock <co> CTO "주제" · /mock <co> 순차 "HR,CTO"
---

# /mock — 모의면접 즉시 시작

사용자가 `/mock` 또는 `/mock <인자>` 를 입력하면 인자에 따라 분기. 핵심 차이 — `/interview` 는 **워크스페이스 관리** 가 목적이고, `/mock` 은 **한 회차 모의면접을 바로 돌리는 것** 이 목적이다.

## 인자 파싱

```
/mock                                    → 분기 1 (목록)
/mock <company>                          → 자유 모드, 전체 주제
/mock <company> <mode>                   → 지정 모드, 전체 주제
/mock <company> <mode> <topic>           → 지정 모드, 집중 주제 (단일 파트 모드)
/mock <company> 순차 "<part1>,<part2>"    → 순차 모드, 파트 리스트 지정
/mock <company> 순차                     → 순차 모드, 파트 리스트는 00-context 에서 추출 시도
```

- `<mode>`: `HR` | `기술면접관` | `CTO` | `자유` | `순차` (대소문자·한글 모두 허용)
- `<topic>`: 따옴표로 감싼 집중 주제 문자열 (단일 파트 모드 전용)
- `<parts>`: 따옴표로 감싼 파트 리스트, 콤마 구분 — 예: `"HR,CTO"` / `"팀장,HR,임원"` / `"기술면접관,CTO"`
- mode 없으면 기본 **자유**

## 분기 1: 인자 없음 → 회사 목록 + 최근 Round 표시

```
1. Bash: ls -1 Docs/InterviewPrep/companies/ (디렉토리 목록)
2. 각 회사 폴더에서:
   - 00-context.md 에서 포지션 + 면접 일정 추출
   - 04-mock-interview.md 의 마지막 "## Round N" 헤더 찾아 회차 번호 추출
   - 03-expected-questions.md 의 🔴/🟡 개수 집계
3. 표 형태로 표시:

   | # | 회사 | 포지션 | D-day | 지난 Round | 🔴 | 🟡 |
   |---|------|-------|-------|-----------|----|----|
   | 1 | 111percent | 멀티플레이 엔진 | D-3 (2026-04-20) | Round 4 | 2 | 5 |
   | 2 | devcat | 게임 프레임워크 | 미정 | - | 0 | 3 |

4. 사용자에게: "어느 회사? 번호 또는 이름. 모드는 HR / 기술면접관 / CTO / 자유. 집중 주제 있으면 함께."
```

## 분기 2: 인자 있음 → 회사 존재 확인 + 컨텍스트 패키징

```
1. Bash: ls Docs/InterviewPrep/companies/<company>/ (존재 확인)
   - 없으면: "이 회사는 워크스페이스가 아직 없어. /interview <URL> 로 먼저 만들어야 함." 종료
2. 존재하면 컨텍스트 로딩 (병렬):
   - Read 00-context.md (전체)
   - Read 03-expected-questions.md (전체) — 🔴/🟡 질문 목록 필요
   - Read 04-mock-interview.md (마지막 50줄) — 지난 회차 번호 + 정정 항목
   - Bash: ls Docs/TurnIn/me/ → 파일 목록만 (포트폴리오 요약용)
   - Read Docs/TurnIn/me/<대표 1~2 파일> 첫 50줄 (강점 추출)
   - Bash: git log --oneline -3 -- Docs/InterviewPrep/companies/<company>/ (최근 활동)
3. 다음 Round 번호 계산: (마지막 Round N) + 1
4. (순차 모드 전용) 파트 리스트 결정:
   a. 사용자가 인자로 `"HR,CTO"` 같이 줬으면 → 그대로 사용
   b. 없으면 00-context.md 에서 면접 구성 추출 시도 — 예: "HR 30분 → CTO 30분" 같은 문장·표·리스트
   c. 추출 실패하면 mock-interviewer 가 세션 시작 시 사용자에게 직접 질문
5. mock-interviewer 에이전트에 Task 로 넘김 (아래 "컨텍스트 패키징" 참조)
```

## 컨텍스트 패키징 (mock-interviewer 에 넘김)

### 공통 패키지 (모든 모드)

```
회사: <company>
오늘: <YYYY-MM-DD>
면접 일정: <00-context.md 에서 추출>
D-day: <남은 일수>
모드: <HR | 기술면접관 | CTO | 자유 | 순차>
다음 Round 번호: <N+1>

==========================================
00-context 요약:
<직접 읽은 내용 중 면접 유형·일정·구성·중점 영역만 5~10줄 발췌>

==========================================
03-expected-questions 의 🔴 / 🟡 목록:
🔴:
- <질문1>
- <질문2>
🟡:
- <질문3>
...

==========================================
04-mock-interview 지난 Round 요약:
- 마지막 Round: N (<date>)
- 이번 회차 우선순위 (지난 회차 "다음 회차 우선순위" 섹션에서):
  - [ ] <항목1>
  - [ ] <항목2>

==========================================
사용자 포트폴리오 강점 (TurnIn/me):
- <3~5 bullet>
```

### 모드별 추가 필드

**단일 파트 모드** (`HR` / `기술면접관` / `CTO` / `자유`):
```
집중 주제: <인자로 받았으면 / 없으면 "전체">
```

**순차 모드**:
```
파트 리스트: [<파트1>, <파트2>, ...]  ← 인자 또는 00-context 에서 추출, 없으면 "미지정 — 사용자에게 질문"
파트별 페르소나 매핑:
  - <파트1> → <HR톤 | 기술면접관톤 | CTO톤 | 자유톤 | 알 수 없음 → 사용자 확인 필요>
  - <파트2> → ...
총 시간 예산: 30~60분 (파트 수에 비례)
```

### 공통 지시 (모든 모드)

```
1. 첫 메시지는 1분 브리핑:
   - 단일 파트: 라운드 번호 · 모드 · 문항 수 · 규칙 · "준비되면 '시작'"
   - 순차 파트: 라운드 번호 · 파트 순서 전체 표시 · 각 파트 예상 문항 수 · 전환 규칙 설명 · "준비되면 '시작'"
2. 사용자 "시작" 답하면 질문 개시
3. 각 답변 직후 🟢/🟡/🔴 + 1줄 판정. 장황 금지.
4. 꼬리질문 최소 1개.
5. 순차 모드는 파트 종료 시마다 전환 메시지 + 사용자 "계속" 대기
6. 종료 조건: 예정 문항 소진 OR 사용자 "끝" / "종료" / "여기까지"
7. 종료 시 (a) 세션 요약 출력 (b) 04-mock-interview.md 에 Round <N+1> 섹션 Edit 로 추가 (c) 필요 시 03-expected-questions.md 자신감 분포 갱신
```

## 분기 별 사용 예시

### 예시 1: 목록 → 선택

```
사용자: /mock
→ 표 + "어느 회사?"
사용자: <company> CTO "<주제>"
→ 분기 2 진행, CTO 모드 + 지정 주제 집중
```

### 예시 2: 바로 단일 파트 라운드

```
사용자: /mock <company> HR
→ 컨텍스트 로딩 → mock-interviewer 호출 → HR 모드 Round N+1 브리핑
```

### 예시 3: 특정 취약 주제 드릴

```
사용자: /mock <company> 기술면접관 "<주제>"
→ 지정 주제만 집중, 기술면접관 톤으로 압박, 짧은 회차 (3~5문항, 10~15분)
```

### 예시 4: 순차 모드 — 실제 면접 구조 재현 (파트 명시)

```
사용자: /mock <company> 순차 "HR,CTO"
→ HR 블록 먼저 (대화형·조건/장기성·4~5문항)
→ [전환] "HR 파트 끝. 준비되면 '계속'"
→ 사용자 "계속"
→ CTO 블록 (철학·의사결정·3~4문항)
→ 종합 요약 — 파트별 점수 + 톤 전환 관찰
→ 04 에 "Round N — 순차 (HR→CTO)" 기록
```

### 예시 5: 순차 모드 — 3파트 (00-context 에서 구성 자동 추출)

```
사용자: /mock <company> 순차
→ 00-context.md 에서 면접 구성 파싱 시도
   (예: "팀장 면접 → HR → 임원 면접" 같은 문장/표)
→ 파트 리스트 자동 확정 시 그대로 진행
→ 추출 실패 시 사용자에게 직접 질문: "어떤 파트들? 예: HR,CTO"
```

### 예시 6: 순차 모드 — 커스텀 파트명 (페르소나 확인)

```
사용자: /mock <company> 순차 "임원,실무팀장,HR"
→ "임원" / "실무팀장" / "HR" 매핑:
   - "임원" → CTO톤으로 기본 매핑
   - "실무팀장" → 기술면접관톤으로 기본 매핑
   - "HR" → HR톤
→ 매핑이 애매하면 브리핑 직전에 한 번만 확인:
   "'실무팀장' 은 기술면접관 톤으로 진행할게 — 괜찮으면 '시작', 다른 톤이면 지정해줘"
```

### 예시 4: 특정 취약 주제 드릴

```
사용자: /mock 111percent 기술면접관 "x86 캐시"
→ 캐시/메모리/파이프라인만 집중해서 기술면접관 톤으로 압박
→ 짧게 (문항 3~5개, 10~15분)
```

## 분기 3 (오류): 회사 없음

```
사용자: /mock nexon
→ Bash 확인: Docs/InterviewPrep/companies/nexon/ 없음
→ 응답:
  "nexon 워크스페이스가 아직 없어. 먼저 /interview <모집요강 URL> 로 만들어야 함.
   기존 회사 목록: 111percent, devcat"
```

## 절대 하지 말 것

- ❌ 워크스페이스가 없는 회사에서 모의면접 시작 — 반드시 분기 3
- ❌ mock-interviewer 에 컨텍스트 없이 호출 — "알아서 해봐" 금지, 명시 패키징
- ❌ 03-expected-questions 의 🟢/🟡/🔴 분포를 무시하고 무작위 질문 — 취약 항목부터 파야 훈련 효과
- ❌ 라운드 종료 없이 중간에 멈춤 — 04 에 기록 안 되면 다음 `/mock` 이 진도 파악 불가
- ❌ 튜터 모드로 빠지기 — 설명/가르침은 세션 끝나고 roadmap-tutor 로 명시 이관

## 성공 기준

`/mock <company>` 한 번 입력 → **30초 안에** 브리핑 → 문항 시작. 라운드 끝나면 04-mock-interview.md 에 회차 섹션 추가되고, 🔴 항목 3개 + 다음 학습 액션이 사용자에게 명시됨.
