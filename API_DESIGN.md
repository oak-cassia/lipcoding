# API 설계 문서

## 📋 API 엔드포인트 설계

모든 API는 컨텍스트의 `mentor-mentee-api-spec.md`와 `openapi.yaml` 스펙을 완벽히 준수합니다.

### Base URL
- **개발환경**: `http://localhost:8080/api`
- **문서화**: `http://localhost:8080/swagger`

---

## 🔐 인증 시스템

### POST `/api/signup` - 회원가입

#### Request Body
```json
{
  "email": "user@example.com",
  "password": "password123",
  "name": "김멘토",
  "role": "mentor" // or "mentee"
}
```

#### Response
- `201 Created`: 성공
- `400 Bad Request`: 잘못된 요청 형식
- `500 Internal Server Error`: 서버 오류

#### 구현 포인트
- 이메일 중복 검증
- 비밀번호 BCrypt 해싱
- 역할(mentor/mentee) 검증

### POST `/api/login` - 로그인

#### Request Body
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

#### Response
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### 구현 포인트
- 이메일/비밀번호 검증
- JWT 토큰 생성 (RFC 7519 클레임 포함)
- 1시간 유효기간 설정

---

## 👤 사용자 프로필

### GET `/api/me` - 내 정보 조회

#### Headers
```
Authorization: Bearer <token>
```

#### Response (멘토)
```json
{
  "id": 1,
  "email": "mentor@example.com",
  "role": "mentor",
  "profile": {
    "name": "김멘토",
    "bio": "Frontend mentor",
    "imageUrl": "/api/images/mentor/1",
    "skills": ["React", "Vue"]
  }
}
```

#### Response (멘티)
```json
{
  "id": 10,
  "email": "mentee@example.com",
  "role": "mentee",
  "profile": {
    "name": "이멘티",
    "bio": "Frontend 배우고 싶어요",
    "imageUrl": "/api/images/mentee/10"
  }
}
```

### PUT `/api/profile` - 프로필 수정

#### Request Body (멘토)
```json
{
  "name": "김멘토",
  "bio": "Frontend mentor",
  "image": "BASE64_ENCODED_STRING",
  "skills": ["React", "Vue", "TypeScript"]
}
```

#### Request Body (멘티)
```json
{
  "name": "이멘티",
  "bio": "열심히 배우고 싶습니다",
  "image": "BASE64_ENCODED_STRING"
}
```

### GET `/api/images/{role}/{id}` - 프로필 이미지

#### Response
- `200 OK`: 이미지 바이너리 데이터
- `404 Not Found`: 이미지 없음 (기본 이미지 제공)

#### 기본 이미지 URL
- 멘토: `https://placehold.co/500x500.jpg?text=MENTOR`
- 멘티: `https://placehold.co/500x500.jpg?text=MENTEE`

---

## 👨‍🏫 멘토 관리

### GET `/api/mentors` - 멘토 목록 조회 (멘티 전용)

#### Query Parameters
- `skill`: 기술 스택 필터링 (한 번에 하나만)
- `orderBy`: 정렬 기준 (`skill` | `name`)

#### Examples
```
GET /api/mentors
GET /api/mentors?skill=react
GET /api/mentors?skill=spring&orderBy=name
```

#### Response
```json
[
  {
    "id": 3,
    "email": "mentor1@example.com",
    "role": "mentor",
    "profile": {
      "name": "김앞단",
      "bio": "Frontend mentor",
      "imageUrl": "/api/images/mentor/3",
      "skills": ["React", "Vue"]
    }
  },
  {
    "id": 4,
    "email": "mentor2@example.com", 
    "role": "mentor",
    "profile": {
      "name": "이뒷단",
      "bio": "Backend mentor",
      "imageUrl": "/api/images/mentor/4",
      "skills": ["Spring Boot", "FastAPI"]
    }
  }
]
```

#### 구현 포인트
- 멘티만 접근 가능 (역할 검증)
- 기술 스택 JSON 배열 검색
- 이름/기술 스택 기준 정렬

---

## 🤝 매칭 요청 관리

### POST `/api/match-requests` - 매칭 요청 보내기 (멘티 전용)

#### Request Body
```json
{
  "mentorId": 3,
  "message": "멘토링 받고 싶어요!"
}
```

#### Response
```json
{
  "id": 1,
  "mentorId": 3,
  "menteeId": 4,
  "message": "멘토링 받고 싶어요!",
  "status": "pending"
}
```

#### 구현 포인트
- 중복 요청 방지 (같은 멘토에게)
- 멘티는 동시에 여러 멘토에게 요청 불가 (pending 상태 확인)

### GET `/api/match-requests/incoming` - 받은 요청 목록 (멘토 전용)

#### Response
```json
[
  {
    "id": 11,
    "mentorId": 5,
    "menteeId": 1,
    "message": "멘토링 받고 싶어요!",
    "status": "pending"
  },
  {
    "id": 12,
    "mentorId": 5,
    "menteeId": 2,
    "message": "React 배우고 싶습니다",
    "status": "accepted"
  }
]
```

### GET `/api/match-requests/outgoing` - 보낸 요청 목록 (멘티 전용)

#### Response
```json
[
  {
    "id": 11,
    "mentorId": 1,
    "menteeId": 10,
    "status": "pending"
  },
  {
    "id": 12,
    "mentorId": 2,
    "menteeId": 10,
    "status": "accepted"
  }
]
```

### PUT `/api/match-requests/{id}/accept` - 요청 수락 (멘토 전용)

#### Response
```json
{
  "id": 11,
  "mentorId": 2,
  "menteeId": 1,
  "message": "멘토링 받고 싶어요!",
  "status": "accepted"
}
```

#### 구현 포인트
- 멘토는 한 번에 한 명의 멘티만 수락 가능
- 수락 시 다른 pending 요청들 자동 거절

### PUT `/api/match-requests/{id}/reject` - 요청 거절 (멘토 전용)

### DELETE `/api/match-requests/{id}` - 요청 취소 (멘티 전용)

---

## 🔒 보안 및 인증

### JWT 토큰 구조 (RFC 7519 준수)

```json
{
  "iss": "mentor-mentee-app",
  "sub": "1",
  "aud": "mentor-mentee-app-users", 
  "exp": 1640995200,
  "nbf": 1640991600,
  "iat": 1640991600,
  "jti": "550e8400-e29b-41d4-a716-446655440000",
  "name": "김멘토",
  "email": "mentor@example.com",
  "role": "mentor"
}
```

### 인증 헤더
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 오류 응답 형식
```json
{
  "error": "Unauthorized",
  "message": "Invalid or expired token",
  "timestamp": "2025-06-28T10:30:00Z"
}
```

---

## 📤 HTTP 상태 코드 매핑

### 성공 응답
- `200 OK`: 조회, 수정 성공
- `201 Created`: 생성 성공

### 클라이언트 오류
- `400 Bad Request`: 잘못된 요청 형식
- `401 Unauthorized`: 인증 실패
- `403 Forbidden`: 권한 부족
- `404 Not Found`: 리소스 없음

### 서버 오류
- `500 Internal Server Error`: 서버 내부 오류

---

## 🔧 구현 우선순위

### 1. 인증 시스템
- [x] 회원가입/로그인 API
- [x] JWT 토큰 생성/검증
- [x] 미들웨어 인증 처리

### 2. 사용자 관리
- [x] 프로필 조회/수정
- [x] 이미지 업로드/서빙
- [x] 역할 기반 접근 제어

### 3. 멘토 관리
- [x] 멘토 목록 조회
- [x] 검색 및 정렬 기능

### 4. 매칭 시스템
- [x] 요청 생성/조회
- [x] 수락/거절/취소
- [x] 상태 관리

---

## 🧪 테스트 시나리오

### 인증 플로우 테스트
1. 회원가입 → 로그인 → 토큰 검증
2. 잘못된 자격증명으로 로그인 시도
3. 만료된 토큰으로 API 호출

### 매칭 플로우 테스트
1. 멘티 → 멘토 검색 → 요청 전송
2. 멘토 → 요청 확인 → 수락/거절
3. 중복 요청 방지 테스트
4. 권한 기반 접근 제어 테스트
