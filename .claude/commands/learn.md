---
description: Resume learning session — auto-loads recent commits, current date, Roadmap progress, and LEARNING.md, then activates roadmap-tutor agent with full context so user can start learning immediately
allowed-tools: Read, Bash, Glob, Grep, Task
argument-hint: (optional) topic name or chapter — "Ch01", "캐시", "Cpp move semantics", etc.
---

# /learn — 학습 세션 자동 재개

사용자가 `/learn` 또는 `/learn <주제>` 를 입력하면 이 절차로 작동:

## 1. 컨텍스트 자동 로드 (병렬 실행)

다음을 한 번에 병렬로 실행해서 사용자가 어디서 무엇을 하고 있었는지 파악:

```
- Read Docs/LEARNING.md (학습 프로토콜)
- Read Docs/Roadmap/README.md (로드맵 지도)
- Read Docs/Roadmap/Progress.md (사용자 실제 진도 로그)
- Read Docs/Foundations/README.md (Foundations 영역 지도)
- Bash: git log --oneline -10 (최근 활동)
- Bash: git status --short (in-progress 작업)
- Bash: git log --since="3 days ago" --pretty=format:"%h %ad %s" --date=short (최근 3일 활동 시간순)
```

오늘 날짜는 환경 컨텍스트에서 가져옴 (system reminder 의 currentDate 또는 Bash `date`).

## 2. 컨텍스트 분석 (한 단락 요약)

위 정보로 다음 4가지 추론:

1. **마지막 학습 활동**: 가장 최근 커밋 메시지 + Progress.md 의 마지막 일일 로그
2. **현재 위치**: Progress.md 의 "현재 위치" 섹션 (Phase, Chapter, Day in 7-day routine)
3. **공백/막힌 지점**: Progress.md 의 "막힌 지점 누적 기록" 마지막 행
4. **오늘 추천 행동**: LEARNING.md 의 7일 루틴 기준, 마지막 Day 다음으로

`/learn <주제>` 처럼 인자가 있으면:
- 그 주제가 Roadmap 챕터인지 Foundations 영역인지 식별
- 해당 문서로 진입점 고정 (위 추천 무시하고 사용자 명시 우선)

## 3. roadmap-tutor 에이전트 호출

Task 도구로 `roadmap-tutor` 서브에이전트 호출. 다음 정보를 한 번에 패키징해서 prompt 로 넘김:

```
오늘 날짜: <YYYY-MM-DD HH:MM>

마지막 git 활동:
<git log 결과 핵심 3~5줄>

현재 작업 상태:
<git status 결과>

Progress.md 발췌:
- 현재 Phase: <X>
- 현재 Chapter: <Y>
- 7일 루틴 Day: <N>
- 마지막 일일 로그: "<...>"
- 막힌 지점: "<...>"

LEARNING.md 핵심 원칙: <한 줄>

사용자 명시 주제: <인자 있으면, 없으면 "없음">

추천 진입점: <Phase X / Chapter Y / Foundations 영역 또는 사용자 명시>
추천 Day 액션: <Day N+1 의 구체 행동, LEARNING.md 7일 루틴 기준>

지시: 이 컨텍스트로 사용자에게 첫 메시지 작성. "안녕" 같은 인사 X — 바로 본론으로 들어가서 사용자가 어디서 멈췄고 오늘 뭘 할지 한 단락으로 요약 후, 진단 질문 하나 던져. 사용자가 답하면 7일 루틴에 따라 진행.
```

## 4. 에이전트 응답 후 사용자에게 전달

서브에이전트의 응답이 사용자의 첫 화면. 이어지는 대화는 일반 대화 모드로 (서브에이전트가 아닌 main agent 가 roadmap-tutor 의 페르소나로 계속).

> **note**: Task 로 서브에이전트 호출은 일회성. 첫 메시지만 그쪽이 만들고, 후속 대화는 main 이 그 페르소나를 이어감. main 이 [`Docs/.claude/agents/roadmap-tutor.md`](.claude/agents/roadmap-tutor.md) 를 참조해서 톤/규칙 유지.

## 사용 예시

```
사용자: /learn

→ 자동으로:
  - 마지막 커밋: "[feat] add interview-prep-builder generic agent" (2026-04-15)
  - Progress.md: Setup 단계, Ch01 미시작
  - 추천: "Ch01 Win32 Day 1 (백지 리콜) 시작"
  
→ tutor 첫 메시지:
  "지난번 시스템 정리 후 첫 학습 세션이네. Progress.md 보니 Ch01 아직 시작 안 했어.
   Ch01 들어가기 전에 Foundations/SystemsAndOS/01 (콜백) 봤어? 그게 전제야.
   봤으면 Ch01 Day 1 백지 리콜 가자.
   둘 중 하나로 답해줘: (a) 콜백 문서 읽었음 (b) 안 읽었음"
```

```
사용자: /learn 캐시

→ 자동으로:
  - "캐시" → Foundations/ComputerArchitecture/02_Cache_Mechanics.md 로 진입점 고정
  - 마지막 커밋 무시
  - 추천: 해당 문서 Day 1 (정독 + 백지 리콜)
  
→ tutor 첫 메시지:
  "캐시 (Cache Mechanics) 진입. 자료 읽기 전 백지 리콜부터 — 캐시 라인 크기는?
   왜 그 크기인지 한 단락으로."
```

## 절대 하지 말 것

- ❌ 컨텍스트 로드 결과를 사용자에게 다 dump (불필요한 token 낭비)
- ❌ "안녕! 학습 세션 시작할게~" 같은 인사 — 바로 본론
- ❌ 사용자 의향 묻기 전에 30분짜리 강의 생성 — 한 단락 요약 + 질문 1개
- ❌ Progress.md 가 비어있다고 멈추기 — 그러면 "초기 진단 모드" 로 어디부터 시작할지 묻기

## 성공 기준

사용자가 `/learn` 입력 → **30초 안에** "지난번 X 까지 했고, 오늘 Y 하자, A 와 B 중 어느 쪽?" 같은 응답을 받는다. 컨텍스트 다시 설명하느라 5분 소비하지 않는다.
