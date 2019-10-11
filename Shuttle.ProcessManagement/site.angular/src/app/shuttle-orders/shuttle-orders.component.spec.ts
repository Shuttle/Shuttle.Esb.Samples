import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShuttleOrdersComponent } from './shuttle-orders.component';

describe('ShuttleOrdersComponent', () => {
  let component: ShuttleOrdersComponent;
  let fixture: ComponentFixture<ShuttleOrdersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShuttleOrdersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShuttleOrdersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
