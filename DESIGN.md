# 멘토-멘티 매칭 앱 설계 문서

## 📋 프로젝트 개요

### 🎯 핵심 기능
- **멘토-멘티 매칭 시스템**: 멘토와 멘티를 연결하는 플랫폼
- **역할 기반 인증**: JWT를 통한 멘토/멘티 역할   구분
- **매칭 요청 관리**: 요청 생성, 수락/거절, 상태 추적
- **프로필 관리**: 이미지 업로드, 기술 스택 관리

### 기술 스택
- **Backend**: ASP.NET Core 8.0 Web API
- **Frontend**: Blazor Server
- **Database**: SQLite with Entity Framework Core
- **Authentication**: JWT with ASP.NET Core Identity
- **Documentation**: Swagger/OpenAPI (자동 생성)

### 포트 구성
- Frontend (Blazor): `http://localhost:3000`
- Backend (API): `http://localhost:8080`
- API Endpoint: `http://localhost:8080/api`
- Swagger UI: `http://localhost:8080/swagger`

---

## 📁 프로젝트 구조

```
lippy/
├── MentorMenteeApp.sln                     # 솔루션 파일
├── src/
│   ├── MentorMenteeApp.API/                # Web API 프로젝트
│   │   ├── Controllers/                    # API 컨트롤러
│   │   │   ├── AuthController.cs
│   │   │   ├── ProfileController.cs
│   │   │   ├── MentorsController.cs
│   │   │   └── MatchRequestsController.cs
│   │   ├── Models/                         # 데이터 모델
│   │   │   ├── DTOs/                      # Data Transfer Objects
│   │   │   └── Entities/                  # Entity Framework 엔터티
│   │   ├── Services/                       # 비즈니스 로직
│   │   │   ├── IAuthService.cs
│   │   │   ├── AuthService.cs
│   │   │   ├── IUserService.cs
│   │   │   ├── UserService.cs
│   │   │   ├── IMatchRequestService.cs
│   │   │   └── MatchRequestService.cs
│   │   ├── Data/                          # 데이터 액세스
│   │   │   ├── AppDbContext.cs
│   │   │   └── Migrations/
│   │   ├── Middleware/                    # 커스텀 미들웨어
│   │   ├── Configuration/                 # 설정 클래스
│   │   ├── Program.cs                     # 엔트리 포인트
│   │   ├── appsettings.json              # 앱 설정
│   │   └── MentorMenteeApp.API.csproj
│   │
│   ├── MentorMenteeApp.Web/               # Blazor Server 프로젝트
│   │   ├── Components/                    # Blazor 컴포넌트
│   │   │   ├── Layout/
│   │   │   ├── Pages/                    # 페이지 컴포넌트
│   │   │   │   ├── Auth/
│   │   │   │   │   ├── Login.razor
│   │   │   │   │   └── Signup.razor
│   │   │   │   ├── Profile.razor
│   │   │   │   ├── Mentors.razor
│   │   │   │   └── Requests.razor
│   │   │   └── Shared/                   # 공유 컴포넌트
│   │   ├── Services/                     # 클라이언트 서비스
│   │   │   ├── ApiService.cs
│   │   │   ├── AuthService.cs
│   │   │   └── StateService.cs
│   │   ├── Program.cs
│   │   ├── App.razor
│   │   └── MentorMenteeApp.Web.csproj
│   │
│   └── MentorMenteeApp.Shared/            # 공유 라이브러리
│       ├── DTOs/                         # 데이터 전송 객체
│       ├── Models/                       # 공유 모델
│       ├── Constants/
│       └── MentorMenteeApp.Shared.csproj
│
├── tests/
│   └── MentorMenteeApp.Tests/
├── docs/                                 # 설계 문서
│   ├── DESIGN.md
│   ├── API_DESIGN.md
│   └── DATABASE_DESIGN.md
└── README.md
```

---

## 🗄️ 데이터베이스 설계

### User Entity
```csharp
public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Name { get; set; }
    public UserRole Role { get; set; }
    public string? Bio { get; set; }
    public byte[]? ProfileImage { get; set; }
    public string? Skills { get; set; } // JSON 배열로 저장
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<MatchRequest> SentRequests { get; set; } = new List<MatchRequest>();
    public ICollection<MatchRequest> ReceivedRequests { get; set; } = new List<MatchRequest>();
}

public enum UserRole
{
    Mentor,
    Mentee
}
```

### MatchRequest Entity
```csharp
public class MatchRequest
{
    public int Id { get; set; }
    public int MentorId { get; set; }
    public int MenteeId { get; set; }
    public string? Message { get; set; }
    public MatchRequestStatus Status { get; set; } = MatchRequestStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User Mentor { get; set; } = null!;
    public User Mentee { get; set; } = null!;
}

public enum MatchRequestStatus
{
    Pending,
    Accepted,
    Rejected,
    Cancelled
}
```

---

## 🔐 보안 설계

### JWT 클레임 구조 (RFC 7519 준수)
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

### 보안 조치
- **SQL Injection**: Entity Framework Core ORM 사용
- **XSS**: Blazor의 기본 HTML 이스케이핑
- **CSRF**: ASP.NET Core의 기본 CSRF 보호
- **패스워드**: BCrypt 해싱
- **OWASP TOP 10**: 각 취약점별 대응책 적용

---

## 🎨 UI/UX 설계

### 주요 페이지 라우팅
- `/` → 자동 리디렉션 (인증 상태에 따라 `/login` 또는 `/profile`)
- `/login` → 로그인 페이지
- `/signup` → 회원가입 페이지  
- `/profile` → 프로필 관리 페이지
- `/mentors` → 멘토 목록 페이지 (멘티만)
- `/requests` → 요청 관리 페이지 (역할별 다른 뷰)

### 네비게이션 구조
```
인증된 사용자
├── 멘토
│   ├── 프로필 (/profile)
│   └── 요청 관리 (/requests) - 받은 요청 목록
└── 멘티
    ├── 프로필 (/profile)
    ├── 멘토 찾기 (/mentors)
    └── 요청 관리 (/requests) - 보낸 요청 목록
```

---

## 🚀 개발 순서

### Phase 1: 백엔드 기초 구조
1. ASP.NET Core Web API 프로젝트 생성
2. Entity Framework Core 모델 및 DbContext 설정
3. JWT 인증 시스템 구현
4. 기본 컨트롤러 스켈레톤 생성

### Phase 2: API 구현
1. **AuthController**: 회원가입(`/signup`), 로그인(`/login`)
2. **ProfileController**: 내 정보 조회(`/me`), 프로필 수정(`/profile`), 이미지(`/images/:role/:id`)
3. **MentorsController**: 멘토 목록 조회(`/mentors`) with 필터링/정렬
4. **MatchRequestsController**: 매칭 요청 CRUD 및 상태 관리

### Phase 3: Blazor 프론트엔드
1. Blazor Server 프로젝트 초기화
2. 인증 플로우 및 상태 관리 구현
3. 레이아웃 및 네비게이션 컴포넌트
4. 각 페이지 컴포넌트 구현

### Phase 4: 통합 및 테스트
1. API 연동 및 오류 처리
2. UI 테스트 ID 추가 (스펙 요구사항)
3. 이미지 업로드 및 검증 로직
4. E2E 테스트 및 성능 최적화

---

## 📝 스펙 요구사항 체크리스트

### ✅ 기능 요구사항
- [ ] 회원가입 및 로그인 (JWT 토큰)
- [ ] 사용자 프로필 (멘토: 기술스택, 멘티: 기본정보)
- [ ] 멘토 목록 조회 (검색, 정렬)
- [ ] 매칭 요청 기능
- [ ] 요청 수락/거절 (멘토)
- [ ] 요청 목록 조회 및 취소 (멘티)

### ✅ 기술 요구사항
- [ ] 프론트엔드: `http://localhost:3000`
- [ ] 백엔드: `http://localhost:8080`
- [ ] API: `http://localhost:8080/api`
- [ ] OpenAPI 문서 자동 생성
- [ ] Swagger UI 제공
- [ ] JWT 클레임 (RFC 7519 준수)
- [ ] 프로필 이미지 (500x500~1000x1000, 1MB 이하, .jpg/.png)
- [ ] SQLite 데이터베이스

### ✅ 보안 요구사항
- [ ] SQL 인젝션 방지
- [ ] XSS 공격 방지
- [ ] OWASP TOP 10 대응

### ✅ UI 테스트 요구사항
- [ ] 모든 HTML 엘리먼트에 지정된 ID 속성 추가
- [ ] 스펙에 명시된 클래스명 및 데이터 속성

---

## 🔧 실행 명령어

### 개발 환경 실행
```bash
# API 서버 시작 (터미널 1)
cd src/MentorMenteeApp.API
dotnet run

# Blazor 앱 시작 (터미널 2)  
cd src/MentorMenteeApp.Web
dotnet run
```

### 빌드 및 테스트
```bash
# 전체 솔루션 빌드
dotnet build

# 테스트 실행
dotnet test

# EF Core 마이그레이션
dotnet ef migrations add InitialCreate --project src/MentorMenteeApp.API
dotnet ef database update --project src/MentorMenteeApp.API
```
