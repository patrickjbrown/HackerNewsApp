import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppComponent],
      imports: [HttpClientTestingModule]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create the app', () => {
    expect(component).toBeTruthy();
  });

  it('should retrieve stories from the server', () => {
    const mockStories = [
      { id: 1, title: 'Test 1', url: 'https://localhost/', by: 'TestUser1', score: 1, time: '1747630863' },
      { id: 2, title: 'Test 2', url: 'https://localhost/', by: 'TestUser2', score: 2, time: '1747630864' },
    ];

    component.ngOnInit();

    const req = httpMock.expectOne('https://localhost:7176/latest?pageNum=0&pageSize=25');
    expect(req.request.method).toEqual('GET');
    req.flush(mockStories);

    expect(component.stories[0].id).toEqual(1);
    expect(component.stories[1].id).toEqual(2);
  });
});
