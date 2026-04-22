---
description: Domain-aware commit — extends sc:git commit by detecting which area of the learning ecosystem changed (Foundations, Roadmap, InterviewPrep, agents, code, infra) and enriching the commit body with learning-tracking metadata. Output passes the .github/scripts/domains/commits/parser.py validator so DSR / TODO-issue / Project board automation works automatically. Preserves bracket format for GitHub Issue / Project automation.
allowed-tools: Bash, Read, Glob, Grep
argument-hint: (optional) free-text hint about what the commit is — passed through to message generation
---

# /commit — 도메인 인지 커밋 (sc:git 확장 + parser 호환 보장)

기본 [`/sc:git commit`](https://) 의 bracket 형식을 유지하면서, **이 저장소의 commit parser ([`.github/scripts/domains/commits/parser.py`](.github/scripts/domains/commits/parser.py)) 가 모든 섹션을 인식할 수 있도록 형식을 보장** 한다. 결과:

- **git log 만 봐도 학습 여정 추적 가능**
- DSR 자동화가 narrative + body + todo + footer 모두 잡음
- `[Todo]` 항목이 자동으로 GitHub Issue 로 생성됨 (`todo-item` 라벨)
- `Closes #N`, `owner/repo#N`, `GH-N` 자동 추출되어 이슈 자동 마감

## 1. 변경 감지 (parallel)

```bash
git status --short
git diff --stat HEAD
git diff --cached --stat
```

## 2. 도메인 라우팅

각 변경 파일을 다음 표로 분류. 한 커밋이 여러 도메인 걸치면 **가장 비중 큰 것** 을 메인으로 + 부수 도메인은 body 에 명시.

| 경로 패턴 | 도메인 | 권장 bracket | 추가 body 필드 |
|----------|--------|-------------|----------------|
| `Docs/Foundations/<영역>/*.md` | **Foundations** | `[docs]` (신규 흡수) / `[feat]` (이론 신작) | Learning Domain · Source · Connects To |
| `Docs/Foundations/Lab/results.md` | **Foundations Lab** | `[docs]` | Experiment · Measurement · Conclusion |
| `Docs/Roadmap/PhaseN_*/*.md` | **Roadmap Chapter** | `[docs]` | Phase · Chapter · Update Tier (L1~L4) |
| `Docs/Roadmap/{README,Progress,PrereqMap}.md` | **Roadmap Meta** | `[docs]` (Progress) / `[refactor]` (구조) | What changed · Why now |
| `Docs/InterviewPrep/companies/<회사>/*.md` | **Interview Company** | `[docs]` | Company · Workspace Phase · D-day |
| `Docs/InterviewPrep/_template/*.md` | **Interview Template** | `[feat]` (신규) / `[refactor]` (개선) | Template impact (회사 N개 영향) |
| `Docs/InterviewPrep/{README,shared}/*` | **Interview Meta** | `[docs]` 또는 `[feat]` | What changed |
| `Docs/LEARNING.md` | **Learning Method** | `[docs]` | Principle changed |
| `.claude/agents/*.md` | **Agent** | `[feat]` (신규) / `[refactor]` (행동 변경) | Agent · Behavior Change · Trigger Pattern |
| `.claude/commands/*.md` | **Slash Command** | `[feat]` (신규) / `[refactor]` (개선) | Command · Trigger · Auto-loaded Context |
| `.github/scripts/**/*.py` | **Automation Code** | `[feat]`/`[fix]`/`[refactor]` | Module · Behavior · Backward compatibility |
| `src/*.{cpp,h}` | **Engine Code** | 표준 (`[feat]`/`[fix]`/`[refactor]`/`[test]`) | (추가 필드 없음) |
| `shaders/*.hlsl` | **Shader** | `[feat]`/`[fix]` | Shader · VS/PS/CS · Pipeline 영향 |
| `exercises/*.cpp` | **Lab Code** | `[feat]` | Experiment Topic · Hypothesis |
| `CMakeLists.txt`, `build.bat`, `.gitignore` | **Infra** | `[chore]` | (간결) |
| `README.md`, `CLAUDE.md` | **Project Meta** | `[docs]` | What changed |

### Bracket 종류 (sc:git 컨벤션 그대로)

`feat` / `fix` / `docs` / `style` / `refactor` / `test` / `chore` / `design` / `comment` / `rename` / `remove` / `debug` / `!BREAKING CHANGE` / `!HOTFIX`

## 3. 메시지 구조 — parser 가 인식하는 4-section 형식

**중요**: `.github/scripts/domains/commits/parser.py` 의 인식 가능 구조:

```
[type(scope)] title              ← 1줄, 필수

<narrative>                      ← 자유 텍스트 (선택, [Section] 헤더 전 모든 줄)
... 여러 줄 가능 ...

[Body]                           ← 명시적 헤더 (선택)
- key: value
- detail: explanation

[Todo]                           ← 명시적 헤더 (선택, 자동 issue 생성됨)
@category
- task description
- (issue) task that should also become a tracked issue

[Footer]                         ← 명시적 헤더 (선택)
- Closes #123
- Related to anxi77/repo#45
```

### 필드 의미

| 위치 | 용도 | 자동화 활용 |
|------|------|-----------|
| **title** | 한 줄 요약. 60자 이내 권장. | DSR 표시, GitHub UI |
| **narrative** | 변경의 맥락·동기·영향. 단락 단위 자유 작성. | DSR body 의 commit 상세 영역 |
| **[Body]** | 구조화된 변경 사항 (bullet). | DSR `<details>` 안에 표시 |
| **[Todo]** | 후속 작업. `@category` + `- task`. **자동 GitHub Issue 생성**. | `manager.py` → todo-item 라벨 issue |
| **[Footer]** | issue 참조. `Closes #N` / `Fixes #N` / `Related to #N`. | 자동 이슈 마감 (`Closes` 계열) |

## 4. 도메인별 메시지 템플릿

### A. Foundations 도메인

```
[docs] add <Domain>/<Topic> with <intent>

<2~3 단락의 narrative — 왜 이 문서를 추가했고 어떤 학습 흐름의 일부인지>

[Body]
- Learning Domain: <ComputerArchitecture | Cpp | ECS | Networking | ...>
- Source: <Notes/ 흡수 | references/ 흡수 | 신작 | 외부 자료 정리>
- Connects To: <관련 Foundations / Roadmap 챕터 링크 N개>
- Known Gap Filled: <면접 복기 공백 중 어느 것 채움 — 해당 시>

[Todo]
@learning
- (issue) <후속 작성 필요 문서 — 자동으로 GitHub Issue 생성됨>

[Footer]
- Related to #<관련 issue 번호>
```

### B. Roadmap Chapter 도메인

```
[docs] update Ch<NN> <Title> to L<Tier>

<2~3 단락 narrative — 어떤 페다고지컬 변화이고 왜 지금 필요했는지>

[Body]
- Phase: <Phase 번호 + 이름>
- Chapter: <NN — Title>
- Update Tier: L<1|2|3|4>
  L1 = 상단 박스 (전제 + Unity 비유)
  L2 = 직관 섹션 추가 (공식 전 그림/물리)
  L3 = 과제 체크포인트 분해
  L4 = 흔한 함정 트러블슈팅
- Tone Calibration: <시니어 페이스 / 친절 페이스 / 변경 없음>

[Todo]
@roadmap
- <PrereqMap 갱신 필요 시 명시>

[Footer]
- Related to #<chapter issue>
```

### C. Interview Company 도메인

```
[docs] update <company>/<artifact> — <intent>

<narrative — 워크스페이스 진행 단계와 발견된 공백>

[Body]
- Company: <company>
- Workspace Phase: <0 폴더 / 1 회사조사 / 2 컨텍스트 / 3 기술 / 4 질문 / 5 모의·튜터>
- D-day: <남은 일수 또는 N/A>
- Confidence shift: <🔴 N개 → 🟡 N개 → 🟢 N개 변화 — 해당 시>

[Todo]
@interview
- <식별된 보강 주제 — 자동 issue>

[Footer]
- Related to #<company issue>
```

### D. Agent / Command 도메인

```
[feat|refactor] <agent or command name> — <intent>

<narrative — 동작 변화 + 사용자 경험 영향>

[Body]
- Agent/Command: <name>
- Behavior Change: <what's different>
- Trigger Pattern: <어떤 사용자 입력에 활성>
- Auto-loaded Context: <어떤 파일을 미리 읽는지 — command 한정>
- Boundary: <다른 에이전트와 어떻게 분리되는지>

[Footer]
- Closes #<agent task issue — 해당 시>
```

### E. Automation Code 도메인 (.github/scripts)

```
[feat|fix|refactor] <module> — <intent>

<narrative — 왜 변경했고 어떤 자동화 흐름이 영향받는지>

[Body]
- Module: <파일 경로>
- Behavior: <변경된 동작>
- Backward compatibility: <기존 호출자에게 깨지는 것 / 깨지지 않는 것>
- Tested: <어떤 케이스로 검증했는지>

[Todo]
@automation
- <후속 보강 — 해당 시>

[Footer]
- Closes #<automation issue — 해당 시>
```

### F. Engine Code / Shader / Lab Code

표준 sc:git 형식 그대로 + narrative 자유 추가 가능. 학습 컨텍스트 특별 추가 X.

```
[feat] add vertex color interpolation

Triangles now display per-vertex colors via barycentric interpolation
in the rasterizer. This is the smallest visible change before moving
to texture sampling.

[Body]
- Add Vec3 color member to Vertex struct
- Update VS to pass color through to PS
- PS samples interpolated color directly

[Footer]
- Tested with: triangle gradient case
- Closes #<feature issue>
```

### G. Infra (CMake / build.bat / gitignore)

```
[chore] <intent>

<선택적 1~2 줄 narrative>
```

## 5. Parser 호환성 사전 검증 (필수)

메시지 생성 후 사용자 확인 받기 **전에** parser 실행으로 validity 확인:

```bash
cd .github/scripts
python -c "
import sys
sys.path.insert(0, '.')
from domains.commits.parser import CommitParser
p = CommitParser()
msg = '''<생성된 메시지>'''
ok, err = p.validate_format(msg)
if ok:
    d = p.parse(msg)
    print(f'✅ valid — narrative={len(d.narrative)} lines, body={len(d.body)}, todos={len(d.todos)}, footer={len(d.footer)}')
    refs = p.extract_issue_references(d)
    print(f'   refs: closes={refs[\"closes\"]} related={refs[\"related\"]}')
else:
    print(f'❌ invalid: {err}')
"
```

검증 결과를 사용자에게 보여주고 확인.

## 6. 대화형 확인

```
당신: 다음 메시지로 커밋할게:

  [docs] add Cpp/03_RAII_And_Smart_Pointers.md with DX11 ComPtr context

  Introduces RAII as the answer to manual new/delete pain. Includes
  ComPtr<T> as the DX11 application of the pattern, prepping for
  Ch04 entry.

  Source absorbed from Notes/'s RAII preview section + cross-referenced
  with Foundations/Cpp/01 and 02 for continuity.

  [Body]
  - Learning Domain: Cpp
  - Source: 신작 (Notes 의 RAII 미리보기 섹션 확장)
  - Connects To:
    - ../Cpp/01_Pointers_References_Lifetime.md
    - ../Cpp/02_Lvalue_Rvalue_And_Move.md
    - Roadmap Ch04 (DX11 First Triangle)
  - Known Gap Filled: 네이티브 C++ 수명·소유권 (interview retrospective)

  [Todo]
  @learning
  - (issue) write Foundations/Cpp/04_Templates_And_Constexpr.md
  - (issue) write Foundations/Cpp/07_COM_And_Reference_Counting.md

  [Footer]
  - Related to anxi77/dx11learning#42

✅ Parser validation: valid — narrative=2 lines, body=4, todos=2, footer=1
   refs: closes=[] related=['anxi77/dx11learning#42']

OK / 수정 / 다시 / 분할
```

- **OK**: 그대로 commit
- **수정**: 사용자가 수정 사항 명시 → 반영 후 재확인
- **다시**: 메시지 새로 생성
- **분할**: 변경을 여러 atomic commit 으로 (도메인별로)

## 7. Atomic Commit 분할 권고

다음 경우 분할 권장:

- 도메인이 3개 이상 섞임
- 코드 변경 + 문서 변경 동시
- Agent/Command 변경 + 그 외

분할 시 `git add <path>` 로 staging 후 commit 반복.

## 8. 실행

확인된 메시지로:

```bash
git add <staged paths>
git commit -m "$(cat <<'EOF'
<message>
EOF
)"
```

성공 후 `git log --oneline -3` 으로 결과 표시.

## 9. 절대 하지 말 것

- ❌ Claude 메시지 ("Co-Authored-By: Claude" 등) — sc:git 컨벤션 명시적 금지
- ❌ 도메인 분류 추측만으로 진행 — 애매하면 사용자에게 묻기
- ❌ 학습 컨텍스트 필드를 코드 커밋에 강제 (노이즈)
- ❌ "init" 같은 의미 없는 메시지
- ❌ amend (`--amend`) — 새 커밋 만들기 (user instruction)
- ❌ `--no-verify` (hooks 우회) — hooks 가 실패하면 원인 수정
- ❌ `[Todo]` 섹션 안에 카테고리 없이 bullet 직접 — `@category` 필수 (parser 가 'General' 로 fallback 하지만 명시 권장)
- ❌ Footer 의 issue ref 형식 위반 — `Closes/Fixes/Resolves/Related to/See/Refs` 키워드 필수 (그래야 parser 가 추출)

## 10. 성공 기준

git log 한 줄만 봐도:
- 어느 도메인이 변경됐는지 (`[docs]` + 첫 단어)
- 무엇을 했는지 (subject)

본문 5초 스캔으로:
- 어느 챕터/회사/Foundations 영역인지
- 면접 공백을 채웠는지 (해당 시)
- 다른 어디와 연결되는지

GitHub 자동화 호환:
- bracket type 으로 라벨 매핑 가능
- `[Todo]` → 자동 GitHub Issue 생성 (todo-item 라벨)
- `Closes #N` / `owner/repo#N` / `GH-N` → 자동 이슈 마감
- DSR 매일 묶음에 narrative + body 다 표시

## 11. 사용 예시 흐름

```
사용자: /commit

당신:
  ❶ git status --short
     M  Docs/Foundations/Cpp/03_RAII...md  (신규)
     M  Docs/Roadmap/PrereqMap.md          (Cpp/03 추가)
     M  Docs/Roadmap/Phase3_DX11_Core/04_DX11_First_Triangle.md  (PrereqMap 링크 추가)

  ❷ 도메인 감지: Foundations (메인) + Roadmap Meta + Roadmap Chapter
  ❸ 분할 권고: "이 3개를 한 커밋으로 묶을지, atomic 3개로 나눌지?"

사용자: 한 커밋

당신: [위 D 형식 예시처럼 메시지 생성 → parser validation → 확인 → commit]
```
