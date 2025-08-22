import { Injectable } from '@angular/core';


export interface User {
  id: number;
  username: string;
  email: string;
  password?: string; // ? значит поле не обязательно
}

