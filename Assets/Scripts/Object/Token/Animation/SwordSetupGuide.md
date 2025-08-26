# 칼 프리팹 설정 가이드

## 1. 칼 프리팹 생성

### 1.1 기본 구조
```
SwordPrefab (GameObject)
├── SwordSprite (SpriteRenderer)
├── TrailRenderer (선택사항)
└── SwordEffect (Script)
```

### 1.2 설정 단계

1. **빈 GameObject 생성**
   - 이름: "SwordPrefab"
   - 위치: Assets/Prefabs/Token/

2. **SpriteRenderer 추가**
   - 칼 스프라이트 할당
   - Sorting Layer: "Effects" 또는 적절한 레이어
   - Order in Layer: 10

3. **TrailRenderer 추가 (선택사항)**
   - 칼 휘두르는 효과를 위한 트레일
   - Material: 기본 트레일 머티리얼
   - Width: 0.1f
   - Time: 0.2f

4. **SwordEffect 스크립트 추가**
   - lifetime: 0.5f
   - swordColor: 흰색 또는 원하는 색상
   - swordScale: 1f

## 2. TokenMovement 설정

### 2.1 Inspector 설정
- **Sword Prefab**: 생성한 SwordPrefab 할당
- **Sword Spawn Point**: 칼이 생성될 위치 (선택사항)
- **Sword Swing Duration**: 0.3f
- **Sword Swing Angle**: 90f

### 2.2 Sword Spawn Point 설정
- Token의 자식으로 빈 GameObject 생성
- 이름: "SwordSpawnPoint"
- 위치: 토큰 앞쪽 (예: Vector3(0, 1, 1))

## 3. 테스트 기능

### 3.1 테스트 모드 활성화
- TokenMovement의 Inspector에서 **Enable Test Mode** 체크
- **Test Target Position** 설정 (기본값: Vector3(2, 0, 2))

### 3.2 테스트 실행
1. 게임 실행
2. 토큰이 생성된 후 **K키** 누르기
3. 콘솔에서 로그 확인:
   - "테스트: 칼 휘두르기 애니메이션 실행"
   - "칼 휘두르기 히트!"
   - "칼 휘두르기 완료!"

### 3.3 자동 테스트 칼 생성
- TestSwordCreator 스크립트를 빈 GameObject에 추가
- 게임 시작 시 자동으로 테스트용 칼 생성
- 생성된 칼을 TokenMovement의 Sword Prefab에 할당

## 4. 애니메이션 동작

### 4.1 근접 공격 시퀀스
1. 토큰이 타겟을 향해 회전 (0.2초)
2. 칼 생성 및 등장 효과 (0.1초)
3. 왼쪽에서 오른쪽으로 휘두르기 (0.18초)
4. 히트 효과 (스케일 변화)
5. 칼 회수 및 사라짐 (0.12초)
6. 토큰 원위치 복귀 (0.3초)

### 4.2 시각적 효과
- **등장**: OutBack 이징으로 부드러운 등장
- **휘두르기**: OutQuart 이징으로 빠른 휘두르기
- **히트**: 스케일 변화로 임팩트 표현
- **사라짐**: InBack 이징으로 부드러운 사라짐

## 5. 트러블슈팅

### 5.1 칼이 보이지 않는 경우
- Sorting Layer 확인
- Order in Layer 확인
- 스프라이트 할당 확인
- Sword Prefab이 TokenMovement에 할당되었는지 확인

### 5.2 애니메이션이 부자연스러운 경우
- swordSwingDuration 조정
- 이징 타입 변경
- 각도 값 조정

### 5.3 테스트가 작동하지 않는 경우
- Enable Test Mode가 체크되어 있는지 확인
- K키 입력이 제대로 감지되는지 확인
- 콘솔 로그 확인

### 5.4 성능 최적화
- Object Pooling 적용 고려
- 불필요한 컴포넌트 제거
- LOD 시스템 적용 고려 