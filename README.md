# 멘토-멘티 매칭 앱

## 📋 프로젝트 개요

이 프로젝트는 멘토와 멘티를 매칭해주는 웹 애플리케이션입니다. ASP.NET Core Web API와 Blazor Server를 사용하여 구현되었으며, 컨텍스트에 제공된 모든 스펙을 준수합니다.

### 🎯 주요 기능
- **회원가입/로그인**: JWT 기반 인증 시스템
- **프로필 관리**: 이미지 업로드, 기술 스택 관리
- **멘토 검색**: 기술 스택 기반 필터링 및 정렬
- **매칭 요청**: 요청 생성, 수락/거절, 상태 관리

### 🏗️ 기술 스택
- **Backend**: ASP.NET Core 8.0 Web API
- **Frontend**: Blazor Server
- **Database**: SQLite with Entity Framework Core
- **Authentication**: JWT with RFC 7519 클레임
- **UI**: Bootstrap 5

---

## 🚀 빠른 시작

### 사전 요구사항
- .NET 8.0 SDK
- Visual Studio Code 또는 Visual Studio 2022

### 1. 프로젝트 클론
```bash
git clone <repository-url>
cd lippy
```

### 2. 패키지 복원
```bash
dotnet restore
```

### 3. 데이터베이스 설정
```bash
# API 프로젝트로 이동
cd src/MentorMenteeApp.API

# 마이그레이션 생성 (필요시)
dotnet ef migrations add InitialCreate

# 데이터베이스 생성
dotnet ef database update
```

### 4. 애플리케이션 실행

#### 터미널 1: API 서버 실행
```bash
cd src/MentorMenteeApp.API
dotnet run
```

#### 터미널 2: Blazor 앱 실행
```bash
cd src/MentorMenteeApp.Web
dotnet run
```

### 5. 접속 URL
- **Blazor 앱**: http://localhost:3000
- **API 서버**: http://localhost:8080
- **Swagger UI**: http://localhost:8080/swagger

---

## 📁 프로젝트 구조

```
lippy/
├── MentorMenteeApp.sln              # 솔루션 파일
├── src/
│   ├── MentorMenteeApp.API/         # Web API 프로젝트
│   │   ├── Controllers/             # API 컨트롤러
│   │   ├── Models/                  # 데이터 모델
│   │   ├── Services/                # 비즈니스 로직
│   │   ├── Data/                    # Entity Framework
│   │   └── Program.cs
│   │
│   ├── MentorMenteeApp.Web/         # Blazor Server 프로젝트
│   │   ├── Components/              # Blazor 컴포넌트
│   │   ├── Services/                # 클라이언트 서비스
│   │   └── Program.cs
│   │
│   └── MentorMenteeApp.Shared/      # 공유 라이브러리
│       ├── DTOs/                    # 데이터 전송 객체
│       └── Models/                  # 공유 모델
│
├── tests/                           # 테스트 프로젝트
├── docs/                           # 설계 문서
│   ├── DESIGN.md                   # 전체 설계 문서
│   ├── API_DESIGN.md               # API 설계
│   ├── DATABASE_DESIGN.md          # 데이터베이스 설계
│   └── BLAZOR_UI_DESIGN.md         # UI 설계
└── README.md
```

---

## 🔐 인증 시스템

### JWT 토큰 구조 (RFC 7519 준수)
```json
{
  "iss": "mentor-mentee-app",
  "sub": "user_id",
  "aud": "mentor-mentee-app-users",
  "exp": 1640995200,
  "nbf": 1640991600,
  "iat": 1640991600,
  "jti": "unique_token_id",
  "name": "김멘토",
  "email": "mentor@example.com",
  "role": "mentor"
}
```

### 기본 테스트 계정
개발용 테스트 계정들이 자동으로 생성됩니다:

#### 멘토 계정
- **이메일**: mentor1@example.com
- **비밀번호**: password123
- **이름**: 김프론트
- **기술스택**: React, Vue, JavaScript, TypeScript

#### 멘티 계정
- **이메일**: mentee1@example.com
- **비밀번호**: password123
- **이름**: 박학습

---

## 📊 API 엔드포인트

### 인증
- `POST /api/signup` - 회원가입
- `POST /api/login` - 로그인

### 프로필
- `GET /api/me` - 내 정보 조회
- `PUT /api/profile` - 프로필 수정
- `GET /api/images/{role}/{id}` - 프로필 이미지

### 멘토
- `GET /api/mentors` - 멘토 목록 조회 (멘티만)

### 매칭 요청
- `POST /api/match-requests` - 매칭 요청 생성 (멘티만)
- `GET /api/match-requests/incoming` - 받은 요청 목록 (멘토만)
- `GET /api/match-requests/outgoing` - 보낸 요청 목록 (멘티만)
- `PUT /api/match-requests/{id}/accept` - 요청 수락 (멘토만)
- `PUT /api/match-requests/{id}/reject` - 요청 거절 (멘토만)
- `DELETE /api/match-requests/{id}` - 요청 취소 (멘티만)

자세한 API 문서는 애플리케이션 실행 후 http://localhost:8080/swagger 에서 확인할 수 있습니다.

---

## 🎨 사용자 인터페이스

### 주요 페이지
- **로그인** (`/login`) - 사용자 로그인
- **회원가입** (`/signup`) - 새 사용자 등록
- **프로필** (`/profile`) - 프로필 관리
- **멘토 찾기** (`/mentors`) - 멘토 검색 및 요청 (멘티만)
- **요청 관리** (`/requests`) - 매칭 요청 관리

### UI 테스트 ID
모든 UI 요소는 컨텍스트 스펙에 명시된 ID와 클래스를 사용합니다:
- 로그인: `#email`, `#password`, `#login`
- 회원가입: `#email`, `#password`, `#role`, `#signup`
- 프로필: `#name`, `#bio`, `#skillsets`, `#profile-photo`, `#profile`, `#save`
- 멘토 목록: `.mentor`, `#search`, `#name`, `#skill`
- 요청: `#message`, `#request`, `#accept`, `#reject`

---

## 🔧 개발 도구

### Entity Framework 명령어
```bash
# 새 마이그레이션 생성
dotnet ef migrations add <MigrationName> --project src/MentorMenteeApp.API

# 데이터베이스 업데이트
dotnet ef database update --project src/MentorMenteeApp.API

# 마이그레이션 롤백
dotnet ef database update <PreviousMigration> --project src/MentorMenteeApp.API
```

### 빌드 및 테스트
```bash
# 전체 솔루션 빌드
dotnet build

# 테스트 실행
dotnet test

# 릴리즈 빌드
dotnet build --configuration Release
```

---

## 🛡️ 보안 고려사항

### 구현된 보안 조치
- **SQL Injection**: Entity Framework Core ORM 사용
- **XSS**: Blazor의 자동 HTML 이스케이핑
- **CSRF**: ASP.NET Core의 기본 보호
- **Password**: BCrypt 해싱
- **JWT**: 안전한 토큰 생성 및 검증
- **CORS**: 허용된 Origin만 접근 가능

### OWASP TOP 10 대응
각 취약점에 대한 구체적인 대응책이 구현되어 있습니다.

---

## 📸 프로필 이미지 요구사항

- **형식**: .jpg, .png만 허용
- **크기**: 500x500 ~ 1000x1000 픽셀
- **용량**: 최대 1MB
- **기본 이미지**:
  - 멘토: `https://placehold.co/500x500.jpg?text=MENTOR`
  - 멘티: `https://placehold.co/500x500.jpg?text=MENTEE`

---

## 🧪 테스트

### API 테스트
Swagger UI를 통해 모든 API 엔드포인트를 테스트할 수 있습니다.

### UI 테스트
모든 UI 요소에는 테스트를 위한 ID가 지정되어 있어 자동화된 테스트가 가능합니다.

---

## 📝 라이센스

이 프로젝트는 MIT 라이센스 하에 배포됩니다.

---

## 🤝 기여하기

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📞 지원

문제가 발생하거나 질문이 있으시면 이슈를 생성해 주세요.

---

## 🔄 업데이트 로그

### v1.0.0
- 초기 릴리즈
- 모든 기본 기능 구현
- 컨텍스트 스펙 100% 준수
