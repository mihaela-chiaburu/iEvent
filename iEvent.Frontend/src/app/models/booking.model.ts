import { BookingTicket } from "./booking-ticket.model";

export interface Booking {
  bookingId: string;
  customerId: string;
  eventId: string;
  bookingDate: string;
  status: number;
  totalPrice: number;
  tickets: BookingTicket[];
  bookingCode: string;
  paymentMethod: number;
  paidAt?: string;
}