# 백엔드 개발 프로세스

## 🎯 개발 목표
- ASP.NET Core 8.0 Web API 구현
- 컨텍스트 스펙 100% 준수  
- JWT 인증 시스템 구현
- Entity Framework Core + SQLite 사용
- OpenAPI/Swagger 자동 문서화

---

## 📋 Phase 1: 프로젝트 기초 설정 (15분)

### 1.1 Web API 프로젝트 생성
```bash
# Web API 프로젝트 생성
dotnet new webapi -n MentorMenteeApp.API
cd MentorMenteeApp.API
```

### 1.2 NuGet 패키지 설치
```bash
# Entity Framework Core
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design

# Authentication & JWT
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

# Password Hashing
dotnet add package BCrypt.Net-Next
```

---

## 📋 Phase 2: 데이터 모델 및 DbContext 구현 (30분)

### 2.1 Enum 및 DTO 생성
**파일 생성 순서:**
1. `Enums/UserRole.cs`
2. `Enums/MatchRequestStatus.cs`
3. `DTOs/Auth/LoginRequest.cs`
4. `DTOs/Auth/SignupRequest.cs`
5. `DTOs/Auth/LoginResponse.cs`
6. `DTOs/User/UserProfileResponse.cs`
7. `DTOs/User/UpdateProfileRequest.cs`

### 2.2 Entity 모델 생성
**파일 생성 순서:**
1. `Models/Entities/User.cs`
2. `Models/Entities/MatchRequest.cs`

### 2.3 DbContext 구현
**파일 생성:**
1. `Data/AppDbContext.cs`

### 2.4 마이그레이션 생성
```bash
dotnet ef migrations add InitialCreate
```

---

## 📋 Phase 3: 인증 시스템 구현 (45분)

### 3.1 JWT 서비스 구현
**파일 생성 순서:**
1. `Configuration/JwtSettings.cs`
2. `Services/Interfaces/IJwtService.cs`
3. `Services/JwtService.cs`

### 3.2 인증 서비스 구현
**파일 생성 순서:**
1. `Services/Interfaces/IAuthService.cs`
2. `Services/AuthService.cs`

### 3.3 AuthController 구현
**파일 생성:**
1. `Controllers/AuthController.cs`

### 3.4 Program.cs 기본 설정
- JWT 인증 미들웨어 설정
- CORS 설정
- Swagger 설정
- 서비스 등록

---

## 📋 Phase 4: 사용자 관리 API 구현 (30분)

### 4.1 사용자 서비스 구현
**파일 생성 순서:**
1. `Services/Interfaces/IUserService.cs`
2. `Services/UserService.cs`

### 4.2 ProfileController 구현
**파일 생성:**
1. `Controllers/ProfileController.cs`
- 내 정보 조회 (`GET /api/me`)
- 프로필 수정 (`PUT /api/profile`)
- 프로필 이미지 서빙 (`GET /api/images/{role}/{id}`)

---

## 📋 Phase 5: 멘토 관리 API 구현 (20분)

### 5.1 멘토 서비스 구현
**파일 생성 순서:**
1. `Services/Interfaces/IMentorService.cs`
2. `Services/MentorService.cs`

### 5.2 MentorsController 구현
**파일 생성:**
1. `Controllers/MentorsController.cs`
- 멘토 목록 조회 (`GET /api/mentors`)
- 기술 스택 필터링 및 정렬 기능

---

## 📋 Phase 6: 매칭 요청 API 구현 (45분)

### 6.1 매칭 요청 서비스 구현
**파일 생성 순서:**
1. `Services/Interfaces/IMatchRequestService.cs`
2. `Services/MatchRequestService.cs`

### 6.2 MatchRequestsController 구현
**파일 생성:**
1. `Controllers/MatchRequestsController.cs`
- 매칭 요청 생성, 수락, 거절, 취소
- 받은/보낸 요청 목록 조회

---

## 📋 Phase 7: 데이터베이스 초기화 및 시드 데이터 (10분)

### 7.1 간단한 시드 데이터 생성
**파일 생성:**
1. `Data/DbInitializer.cs`
- 기본 멘토/멘티 계정 2-3개씩 생성

### 7.2 Program.cs 완성
- 데이터베이스 초기화 및 시드 데이터 실행
- 루트 경로 Swagger 리디렉션

---

## 📋 Phase 8: 최종 테스트 (10분)

### 8.1 API 테스트
- Swagger UI에서 주요 엔드포인트 테스트
- JWT 토큰 검증

### 8.2 최종 실행 명령어 확인
```bash
# 마이그레이션 적용
dotnet ef database update

# API 서버 실행
dotnet run --urls "http://localhost:8080"
```

---

## ✅ 완성 체크리스트

### 필수 API 엔드포인트
- [ ] `POST /api/signup` - 회원가입
- [ ] `POST /api/login` - 로그인  
- [ ] `GET /api/me` - 내 정보 조회
- [ ] `PUT /api/profile` - 프로필 수정
- [ ] `GET /api/images/{role}/{id}` - 프로필 이미지
- [ ] `GET /api/mentors` - 멘토 목록 조회
- [ ] `POST /api/match-requests` - 매칭 요청 생성
- [ ] `GET /api/match-requests/incoming` - 받은 요청 목록
- [ ] `GET /api/match-requests/outgoing` - 보낸 요청 목록
- [ ] `PUT /api/match-requests/{id}/accept` - 요청 수락
- [ ] `PUT /api/match-requests/{id}/reject` - 요청 거절
- [ ] `DELETE /api/match-requests/{id}` - 요청 취소

### 기술 요구사항
- [ ] `http://localhost:8080` → Swagger UI 리디렉션
- [ ] JWT 토큰 (RFC 7519 클레임 포함)
- [ ] 프로필 이미지 제약조건 (jpg/png, 500-1000px, 1MB)
- [ ] SQLite 데이터베이스 자동 생성

---

## 🚀 개발 팁

### 개발 순서 우선순위
1. **인증 시스템 먼저** - 모든 API가 의존하는 기본 기능
2. **데이터 모델 완성** - 변경 시 마이그레이션 필요
3. **기본 CRUD 구현** - 복잡한 비즈니스 로직 전에
4. **비즈니스 규칙 적용** - 마지막에 세부 로직

### 디버깅 전략
- Swagger UI를 활용한 실시간 API 테스트
- 로그 레벨을 Debug로 설정하여 상세 로그 확인

이 프로세스를 따라하면 **2시간 30분** 내에 모든 백엔드 기능을 완성할 수 있습니다! 🎯
