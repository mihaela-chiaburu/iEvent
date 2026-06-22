export interface UserTokenPayload {
  email: string;
  role: 'Customer' | 'EventManager' | 'BookingManager' | 'SuperAdmin';
}
