import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShuttleBooksComponent } from './shuttle-books.component';

describe('ShuttleBooksComponent', () => {
  let component: ShuttleBooksComponent;
  let fixture: ComponentFixture<ShuttleBooksComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShuttleBooksComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShuttleBooksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
