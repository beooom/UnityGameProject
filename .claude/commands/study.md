---
description: Generate a low-level study document in Docs/Foundations/ for a given topic — spawns learning-doc-writer agent with Foundations context so the doc follows the library's 11-section template (problem → concept → mechanism → real-world → pitfalls → interview answers → measurement → recall → related) with mermaid-first diagrams and 백지 리콜 checklist
allowed-tools: Read, Bash, Glob, Grep, Task
argument-hint: <주제>   예) /study "CPU 파이프라인" · /study "BVH 트리" · /study "리플리케이션 vs 동기"
---

# /study — Foundations 학습 문서 즉시 생성

사용자가 `/study <주제>` 또는 `/study --retrofit <경로>` 를 입력하면 작동.

## 0. 모드

```
/study <주제>              → 신규 문서 작성 (기본 · 아래 1~5 절차)
/study --retrofit <경로>   → 기존 문서 용어 콜아웃 보강만 (§6)
/study --retrofit-all      → Foundations/ 전체 순회 보강 (§6 대량)
```

## 1. 컨텍스트 자동 로드 (병렬)

```
- Read Docs/LEARNING.md (4관문 프로토콜)
- Read Docs/Foundations/README.md (카테고리 지도)
- Bash: ls -1 Docs/Foundations/ (실제 카테고리 목록)
- Bash: ls Docs/Foundations/*/README.md 2>/dev/null (카테고리별 README 목록)
- Bash: date +%Y-%m-%d (오늘 날짜)
- Bash: git log --oneline -5 -- Docs/Foundations/ (최근 학습 활동)
```

## 2. 주제 분석 & 카테고리 후보 결정

받은 `<주제>` 에 대해:

1. **키워드 → 카테고리 매핑** 시도:
   - "캐시·DRAM·파이프라인·MESI·메모리 정렬" → `ComputerArchitecture/`
   - "GC·struct·Burst·Allocator·unsafe·NativeContainer" → `CSharp/`
   - "포인터·RAII·move·템플릿·COM" → `Cpp/`
   - "Archetype·Chunk·Job·ISystem·Entity" → `ECS/`
   - "렌더 파이프라인·셰이더·GPU·Compute" → `Graphics/`
   - "선형대수·삼각함수·이산수학·BVH" → `Math/`
   - "결정론·AoI·예측·리플리케이션·Quantum" → `Networking/`
   - "A*·BT·좌표 압축·자료구조" → `Algorithms/`
   - "DSP·WAV·3D 사운드" → `Audio/`
   - "BridgeSystem·SDK Loader·Editor 패턴" → `Architecture/`
   - "Win32·콜백·프로세스·스레드·IoC" → `SystemsAndOS/`
   - "UI Toolkit·Editor 확장" → `Tools/`
2. **모호하거나 복수 후보**면 learning-doc-writer 에게 넘겨서 범위 질문 시키기
3. **매칭 실패**면 새 카테고리 제안하도록 learning-doc-writer 에 지시

## 3. 중복 검사

```
Bash: find Docs/Foundations/<추정카테고리>/ -name "*.md" (파일 목록)
Grep: 주제 핵심 키워드 — 이미 다루는 문서 있는지
```

중복 발견 시 learning-doc-writer 에게 "기존 문서 보강 / 별도 파일 / 재작성" 분기 결정 위임.

## 4. learning-doc-writer 에이전트 호출 (Task)

다음을 패키징해서 prompt 로 넘김:

```
주제: <사용자 원본 문자열>
오늘: <YYYY-MM-DD>

==========================================
카테고리 후보 (우선순위 순):
1. <1순위 카테고리> — <왜>
2. <2순위 카테고리> — <왜>
(모호하면 범위 질문 1번만)

==========================================
기존 Foundations 구조:
<Bash ls 결과 — 카테고리 목록>

==========================================
기존 관련 문서 (중복/인접 검사 결과):
- <파일 경로>: <한 줄 요약>
- ...
(없으면 "관련 기존 문서 없음")

==========================================
사용자 학습 맥락:
- 1차 면접(2026-04-14)에서 공백 드러난 영역: GC 세대 / BlobData / x86 / 캐시 / SIMD / 구조체 비트 레이아웃
- 현재 이직 준비 + Roadmap(DX11) + Foundations 3축 병행
- Unity/DOTS/Burst 실무 베이스. C++/저수준은 Notes 시작 단계

==========================================
LEARNING.md 4관문:
1. 백지 리콜  2. 측정·벤치마크  3. Feynman 설명  4. 실제 코드

==========================================
지시:
1. 범위 모호하면 주제 각도 후보 3~4개 제시하고 사용자 확인 1번만
2. 중복 문서 있으면 보강/별도/재작성 분기 확인 1번만
3. 범위 확정 후 템플릿 11섹션 기반으로 작성 (주제에 맞게 섹션 이름·개수 조정)
4. mermaid 우선 — 가능한 모든 도식
5. Unity/DOTS/Burst 연결 섹션 필수
6. 면접 답변 15s/45s/2min 필수
7. 측정 실험 제안 (exercises/) 가능하면 포함
8. 백지 리콜 체크리스트 7~12항목
9. 작성 후 카테고리 README 갱신
10. 최종 보고 — 경로 + 도식 수 + 실험 제안 + 다음 추천 액션
```

## 5. 사용 예시

### 예시 1: 명확한 주제

```
사용자: /study "CPU 파이프라인"
→ 카테고리: ComputerArchitecture
→ 기존: 04_CPU_Pipeline_x64.md 존재
→ writer 가 확인: "기존 문서 있어. 보강/별도/재작성?"
→ 사용자 선택에 따라 진행
```

### 예시 2: 모호한 주제

```
사용자: /study "메모리"
→ writer 가 각도 질문:
  "메모리 너무 넓어. 어느 쪽?
   1. 캐시 계층 (L1~L3, DRAM) → ComputerArchitecture
   2. 가상 메모리 / 페이지 테이블 → SystemsAndOS (신규)
   3. C#/.NET GC 힙 → CSharp
   4. C++ 수명·소유권 → Cpp"
→ 사용자: 2
→ writer 가 새 카테고리 제안 + 작성 진행
```

### 예시 3: 카테고리 없는 주제

```
사용자: /study "셰이더 최적화"
→ Graphics/ 는 있음. 하지만 파일 레벨에선 맞는 게 없음.
→ writer 가 Graphics/Shader_Optimization.md 로 신규 작성
→ Graphics/README.md 에 항목 추가
```

### 예시 4: 신규 카테고리 필요

```
사용자: /study "데이터베이스 인덱스"
→ 기존 카테고리에 안 맞음
→ writer 가 제안:
  "이 주제는 기존 카테고리 밖. 새 폴더 만들까:
   - 제안: Docs/Foundations/Database/
   - 이유: 추후 트랜잭션·정규화도 다룰 여지
   OK? 다른 이름?"
→ 사용자 확인 후 mkdir + README + 문서 생성
```

## 6. Retrofit 모드 — 기존 문서 용어 콜아웃 보강

`/study --retrofit <경로>` 또는 `/study --retrofit-all` 호출 시.

### 6.1 단일 파일

```
1. Read <경로>
2. learning-doc-writer 의 "콜아웃 필수 용어" 리스트와 매칭
   (RISC, CISC, ROB, μop, ISA, ABI, FPU, ALU, AGU, TLB, IMC, NUMA,
    DDR, DRAM, SRAM, SIMD, AVX, SSE, NEON, COM, RAII, RVO, SFINAE,
    CRTP, JIT, AOT, SOA, AOS, LSB, MSB, NDC, RTT, AoI,
    blittable, marshalling, endianness, alignment, coherence, ...)
3. 각 용어의 첫 등장 위치 파악 — 그 위·아래 300자 내에 설명 있는지 확인
4. 설명 없는 것만 콜아웃 삽입 후보로
5. 사용자에게 diff 제시:
   - 삽입 위치 + 삽입 내용
   - "3개 발견: RISC, CISC, μop. 추가할까?" 확인
6. 확인되면 Write (Edit 여러 번 아닌 전체 Write)
```

### 6.2 전체 순회 (`--retrofit-all`)

```
1. Glob Docs/Foundations/**/*.md
2. 각 파일에 대해 §6.1 절차를 dry-run — 삽입 제안 수집
3. 파일별 후보 수 보고:
   | 파일 | 누락 용어 | 개수 |
   |------|----------|------|
   | 04_CPU_Pipeline_x64.md | RISC, CISC, ROB, μop, FPU, ALU, AGU, OoO | 8 |
   | 03_DRAM_And_Memory_Bus.md | NUMA, IMC, UPI, tRCD | 4 |
   | ...
4. 사용자에게: "우선순위 높은 순서로 표시. 어느 파일부터?"
5. 파일별로 §6.1 적용 (사용자 동의 건건)
```

### 6.3 Retrofit 금지 규칙

- ❌ **본문 재작성 금지** — 콜아웃 "추가"만. 기존 문장·코드·표 건드리지 말 것.
- ❌ **중복 삽입 금지** — 이미 다른 형태로 설명돼 있으면 건너뛰기 (e.g. `**RISC**(Reduced Instruction Set Computing)` 괄호 풀이는 콜아웃 대체 불필요)
- ❌ **같은 용어 콜아웃 여러 번** — 한 문서에 첫 등장 1번만
- ❌ **섹션 경계 파괴** — `---` 구분선 사이의 논리 흐름 중간에 끊지 말고, 섹션 시작 직후 또는 해당 문단 끝에 배치

### 6.4 성공 기준

retrofit 실행 후 해당 문서를 모르는 사람이 처음 읽어도 **약어에서 막히지 않고** 읽어 내려갈 수 있어야 함.

---

## 절대 하지 말 것

- ❌ 범위 확정 전에 문서 작성 시작
- ❌ 질문을 2번 이상 던지기 (writer 규칙과 동일)
- ❌ 카테고리 README 갱신 누락 (신규 모드)
- ❌ 한글 파일명 생성
- ❌ `TODO` / 빈 섹션 남기기
- ❌ 명시 요청 없는 Progress.md 기록
- ❌ retrofit 모드에서 본문 수정

## 성공 기준

`/study <주제>` 입력 → **최대 1번 범위 확인** → 문서 생성 → **즉시 학습 가능 상태**. 사용자가 해당 파일을 열어 읽으면 백지 리콜까지 4관문 루프를 바로 시작할 수 있어야 함.

`/study --retrofit <경로>` → 본문 1자도 안 건드리고 누락 약어 콜아웃만 삽입 → diff 보여준 후 확인 → Write.
